using BPMNModel.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http.Metrics;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel.Camunda
{
    public class CamundaExtensions
    {
        private static void GenerateCamundaClasses(Dictionary<string, CamundaType> data)
        {
            var typesToGenerate = new SortedSet<string>();

            foreach (var (name, camundaType) in data)
            {
                if (camundaType.AllowedIn.Any(x => x.StartsWith("bpmn") || x.StartsWith("*")))
                {
                    typesToGenerate.Add(name);
                }
            }

            int size = typesToGenerate.Count;

            do
            {
                size = typesToGenerate.Count;

                foreach (var typeName in typesToGenerate.ToList())
                {
                    var camundaType = data[typeName];

                    if (camundaType.SuperClass is not null && !typesToGenerate.Contains(camundaType.SuperClass) &&
                        camundaType.SuperClass != "Element" && camundaType.SuperClass.StartsWith("camunda"))                     
                    {
                        typesToGenerate.Add(camundaType.SuperClass);
                    }

                    foreach(var (_,(fieldType,_)) in camundaType.Attributes)
                    {
                        var realFieldType = ConvertName(fieldType);

                        if (!typesToGenerate.Contains(realFieldType) &&
                            fieldType != "String" && fieldType != "Integer" && fieldType != "Boolean" && !fieldType.StartsWith("bpmn"))
                        {
                            typesToGenerate.Add(realFieldType);
                        }
                    }

                    foreach (var (_, (elementType,_, _)) in camundaType.ExtensionElements)
                    {
                        var realElementType = ConvertName(elementType);

                        if (!typesToGenerate.Contains(realElementType) &&
                            elementType != "String" && elementType != "Integer" && elementType != "Boolean" && !elementType.StartsWith("bpmn"))
                        {
                            typesToGenerate.Add(realElementType);
                        }
                    }
                }

                foreach (var (_,camundaType) in data.ToList())
                {
                    if (camundaType.SuperClass is not null && typesToGenerate.Contains(camundaType.SuperClass) && !typesToGenerate.Contains(camundaType.Name))
                    {
                        typesToGenerate.Add(camundaType.Name);
                    }
                    //Just checking, not realy adding anything.
                    foreach(var extendsName in camundaType.Extends)
                    {
                        if (typesToGenerate.Contains(extendsName) && !typesToGenerate.Contains(camundaType.Name))
                        {
                            typesToGenerate.Add(camundaType.Name);
                        }
                    }
                    //Just checking, not realy adding anything.
                    foreach (var allowedInName in camundaType.AllowedIn)
                    {
                        if (typesToGenerate.Contains(allowedInName) && !typesToGenerate.Contains(camundaType.Name))
                        {
                            typesToGenerate.Add(camundaType.Name);
                        }
                    }
                }

            } while (typesToGenerate.Count > size);

            var projectdirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent;
            if (projectdirectory == null) throw new BPMNCheckerExceptions("Something went wrong, project structure changed.");
            using var file = new StreamWriter(projectdirectory.FullName + @"\BPMNModel\Camunda\CamundaClasses.cs");

            file.WriteLine("using BPMNModel.Model;");
            file.WriteLine("namespace BPMNModel.Camunda");
            file.WriteLine("{");

            foreach (var typeName in typesToGenerate)
            {
                var camundaType = data[typeName];

                file.WriteLine($"  public {(camundaType.IsAbstract ? "abstract ": "")}{ConvertCSName(camundaType.Name)} {(camundaType.SuperClass is not null?": "+ConvertCSName(camundaType.SuperClass) : "")}");
                file.WriteLine("  {");
                /*
                writer.WriteLine($"Extends: {string.Join(", ", Extends)}");
                writer.WriteLine($"Allowed: {string.Join(", ", AllowedIn)}");
                writer.WriteLine("{");
                if (Attributes.Any())
                {
                    writer.WriteLine("  Attributes:");
                    foreach (var (attName, (attType, attDefault)) in Attributes)
                    {
                        writer.WriteLine($"    {attType} {attName}{(attDefault != null ? " = " + attDefault : "")}");
                    }
                }

                if (ExtensionElements.Any())
                {
                    if (Attributes.Any()) writer.WriteLine();
                    writer.WriteLine("  Extension Elements:");
                    foreach (var (elName, (elType, elIsBody, elIsMany)) in ExtensionElements)
                    {
                        writer.WriteLine($"    {elType} {elName}{(elIsBody ? " (Body)" : "")}{(elIsMany ? " (Many)" : "")}");
                    }
                }
                */
                file.WriteLine("  }\n");
            }


            file.WriteLine("}");
        }

        private static string ConvertCSName(string name)
        {
            if (name == "Element") return name;
            if (name == "String") return "string";
            if (name == "Integer") return "long";
            if (name == "Boolean") return "bool";
            if (name.StartsWith("bpmn")) return name.Replace("bpmn:", "");
            //camunda name
            return name.Replace("camunda:","Camunda");
        }


        private static string ConvertName(string name)
        {
            if (name == "Element") return name;
            if (name.StartsWith("bpmn:") || name.StartsWith("camunda:")) return name;
            return "camunda:" + name;
        }

        public static void EnrichGenerator(ILogger logger, Generator generator)
        {

            var camundaTypes = new Dictionary<string, CamundaType>();

            JsonDocument definitions = ResourcesUtility.LoadResourceAsJsonDocument($@"definitions/camunda.json");

            var typesJSON = definitions.RootElement.GetProperty("types");
            foreach (var typeJSON in typesJSON.EnumerateArray())
            {
                var typeName = typeJSON.GetProperty("name").GetString();
                if (typeName == null) throw new BPMNCheckerExceptions("In camunda.json, all types have a name.");

                bool isAbstract = false;
                if (typeJSON.TryGetProperty("isAbstract", out JsonElement isAbstractProperty))
                {
                    isAbstract = isAbstractProperty.GetBoolean();
                }

                string? superClass = null;
                if (typeJSON.TryGetProperty("superClass", out JsonElement superClassProperty))
                {
                    var superClasses = superClassProperty.EnumerateArray().Select(x => x.GetString()).Where(x => x != null && x != "Element").Cast<string>().ToList();
                    if (superClasses.Count > 1) throw new BPMNCheckerExceptions("In camunda.json, all types have at most one super class.");
                    if (superClasses.Any())
                    {
                        superClass = ConvertName(superClasses[0]);
                    }
                }

                var camundaType = new CamundaType(ConvertName(typeName), isAbstract, superClass);

                if (typeJSON.TryGetProperty("extends", out JsonElement extendsProperty))
                {
                    camundaType.Extends.AddRange(extendsProperty.EnumerateArray().Select(x => x.GetString()).Where(x => x != null).Cast<string>());
                }

                if (typeJSON.TryGetProperty("properties", out JsonElement properties))
                {
                    foreach (var property in properties.EnumerateArray())
                    {
                        if (property.TryGetProperty("isAttr", out JsonElement isAttrProperty))
                        {
                            if (isAttrProperty.GetBoolean() != true) throw new BPMNCheckerExceptions("In camunda.json, all types have at most one super class.");
                            string? attName = property.GetProperty("name").GetString();
                            if (attName == null) throw new BPMNCheckerExceptions("In camunda.json, all attributes should have a name.");
                            string? attType = property.GetProperty("type").GetString();
                            if (attType == null) throw new BPMNCheckerExceptions("In camunda.json, all attributes should have a type.");
                            string? attDefault = null;
                            if (property.TryGetProperty("default", out JsonElement attDefaultProperty))
                            {
                                attDefault = attDefaultProperty.ToString();
                            }

                            camundaType.Attributes.Add(attName, (attType, attDefault));

                            foreach (JsonProperty item in property.EnumerateObject())
                            {
                                if (new[] { "isAttr", "name", "type", "default" }.Contains(item.Name)) continue;
                                Console.WriteLine($"  {item.Name} = {item.Value}");
                            }
                        }
                        else
                        {
                            string? attName = property.GetProperty("name").GetString();
                            if (attName == null) throw new BPMNCheckerExceptions("In camunda.json, all attributes should have a name.");
                            string? attType = property.GetProperty("type").GetString();
                            if (attType == null) throw new BPMNCheckerExceptions("In camunda.json, all attributes should have a type.");

                            bool attIsBody = false;
                            if (property.TryGetProperty("isBody", out JsonElement attIsBodyProperty))
                            {
                                attIsBody = attIsBodyProperty.GetBoolean();
                            }

                            bool attIsMany = false;
                            if (property.TryGetProperty("isMany", out JsonElement attIsManyProperty))
                            {
                                attIsMany = attIsManyProperty.GetBoolean();
                            }

                            camundaType.ExtensionElements.Add(attName, (attType, attIsBody, attIsMany));
                        }
                    }
                }

                if (typeJSON.TryGetProperty("meta", out JsonElement metaProperty))
                {
                    if (metaProperty.TryGetProperty("allowedIn", out JsonElement allowedProperty))
                    {
                        camundaType.AllowedIn.AddRange(allowedProperty.EnumerateArray().Select(x => x.GetString()).Where(x => x != null).Cast<string>());
                    }
                }

                camundaTypes.Add(camundaType.Name, camundaType);
            }
            using var dump = new StreamWriter("camundaProcessed");

            var bpmnCamundaAttributes = new Dictionary<string, List<(string Type, string Name)>>();

            GenerateCamundaClasses(camundaTypes);

            foreach (var (name, camundaType) in camundaTypes)
            {
                //camundaType.Dump(dump);
                var allInInheritance = camundaType.GetAllInInheritance(camundaTypes);
                //if (allInInheritance.Count > 1) dump.WriteLine(">>>");
                //dump.WriteLine(string.Join(", ", allInInheritance));
                var attributes = new List<(string Name, string Type, string? Default)>();
                var extensionElements = new List<(string Name, string Type, bool IsBody, bool IsMany)>();

                // camunda types are sometime extending other camunda types
                var camundaInExtends = camundaTypes.Values.Where(x => x.Extends.Contains(camundaType.Name)).Select(x=>x.Name);
                if (camundaInExtends.Any())
                {
                    allInInheritance.AddRange(camundaInExtends);
                }

                foreach(var typeName in allInInheritance)
                {
                    if (typeName.StartsWith("camunda"))
                    {
                        attributes.AddRange(camundaTypes[typeName].Attributes.Select(x=> (x.Key, x.Value.Type, x.Value.Default)));
                        extensionElements.AddRange(camundaTypes[typeName].ExtensionElements.Select(x=>(x.Key, x.Value.Type, x.Value.IsBody, x.Value.IsMany)));
                    }else
                    {
                        //TODO: bpmn:ErrorEventDefinition
                        attributes.Add(("id", "String", null));
                        attributes.Add(("errorRef", "Error", null));
                    }
                }

                if (camundaType.Extends.Any())
                {
                    //camunda extends were processed, remaining BPMN extends
                    foreach (var extension in camundaType.Extends.Where(x=>!x.StartsWith("camunda")))
                    {
                        if  (extensionElements.Any())
                        {
                            throw new BPMNCheckerExceptions($"In camunda.json, all Camunda types extending BPMN types assumes to have just attributes ({camundaType.Name} have Extension Elements).");
                        }

                        var xmlTypeName = extension.Replace("bpmn:", "t");
                        var xmlType = generator.Types[xmlTypeName];

                        if (xmlType is ComplexType complexType)
                        {
                            var attributesToAdd = new List<(string, string)>();

                            foreach (var (attName, attType, attDefault) in attributes) 
                            {
                                AttributeXMLType attXmlType = attType switch
                                {
                                    "String" => AttributeXMLType.String,
                                    "Boolean" => AttributeXMLType.Boolean,
                                    "Integer" => AttributeXMLType.Integer,
                                    _ => throw new BPMNCheckerExceptions($"In camunda.json, unexpected attribute: {attType} in {camundaType.Name}")
                                };
                                var xmlAttribute = new Attribute(ConvertName(attName), (attXmlType, null));
                                complexType.Attributes.Add(xmlAttribute);

                                attributesToAdd.Add((attType, ConvertName(attName)));
                            }
                            if (bpmnCamundaAttributes.ContainsKey(xmlTypeName))
                            {
                                bpmnCamundaAttributes[xmlTypeName].AddRange(attributesToAdd);
                            }
                            else
                            {
                                bpmnCamundaAttributes.Add(xmlTypeName, attributesToAdd);
                            }

                        }else
                        {
                            throw new BPMNCheckerExceptions($"In camunda.json, {camundaType.Name} is extending wrong type in BMPN.");
                        }
                    }
                }

                if (camundaType.AllowedIn.Any(x => x.StartsWith("camunda")))
                {

                    dump.WriteLine($"{camundaType.Name}  allowed: {string.Join(",", camundaType.AllowedIn)}");
                    if (attributes.Any())
                    {
                        dump.WriteLine("  All Attributes:");
                        foreach (var (attName, attType, attDefault) in attributes)
                        {
                            dump.WriteLine($"    {attType} {attName}{(attDefault != null ? " = " + attDefault : "")}");
                        }
                    }

                    if (extensionElements.Any())
                    {
                        dump.WriteLine("  All Extension Elements:");
                        foreach (var (elName, elType, elIsBody, elIsMany) in extensionElements)
                        {
                            dump.WriteLine($"    {elType} {elName}{(elIsBody ? " (Body)" : "")}{(elIsMany ? " (Many)" : "")}");
                        }
                    }
                    dump.WriteLine();

                }

            }
            /*
            using var toPython = new StreamWriter("camundaProcessed_types");
            toPython.WriteLine("camundaAttributes = {");
            foreach (var (bpmnType, attributes)  in bpmnCamundaAttributes)
            {
                toPython.Write($"  \"{bpmnType.Substring(1)}\" : [");
                toPython.Write(string.Join(", ", attributes.Select(x => $"(\"{x.Type}\",\"{x.Name}\")")));
                toPython.WriteLine("],");
            }
            toPython.WriteLine("}");
            */
        }
    }
}
