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
                //directly allowed in BPMN type
                if (camundaType.AllowedIn.Any(x => x.StartsWith("bpmn") || x.StartsWith("*") || x.StartsWith("camunda")))
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

            file.WriteLine("// -------------------------------------------------------");
            file.WriteLine("// Do not modify directly, this file was generated.");
            file.WriteLine("// -------------------------------------------------------");
            var missing = data.Where(x => !x.Value.IsAbstract).Where(x => !typesToGenerate.Contains(x.Key)).ToList();
            file.WriteLine("// While generating Camunda classes following types were used as if they were abstract:");
            file.WriteLine($"{string.Join("\n",missing.Select(x=>"// "+x.Key))}");
            file.WriteLine("// (Just extending attributes were generated for base types.)");
            file.WriteLine("// -------------------------------------------------------");



            file.WriteLine("using BPMNModel.Model;");
            file.WriteLine();
            file.WriteLine("namespace BPMNModel.Camunda");
            file.WriteLine("{");

            foreach (var typeName in typesToGenerate)
            {
                var camundaType = data[typeName];

                file.Write($"\tpublic {(camundaType.IsAbstract ? "abstract " : "")}class {ConvertCSName(camundaType.Name)}");

                var toExtend = new List<string>();

                if (camundaType.SuperClass is not null)
                {
                    toExtend.Add(ConvertCSName(camundaType.SuperClass));
                }

                if (camundaType.AllowedIn.Any())
                {
                    toExtend.Add("ICamundaBaseElement");
                }

                if (toExtend.Any())
                {
                    file.Write($" : {string.Join(", ", toExtend)}");
                }
                file.WriteLine();
                file.WriteLine("\t{");
                if (camundaType.Extends.Any())
                {
                    file.WriteLine($"\t\t// Extends: {string.Join(", ", camundaType.Extends)}");
                }
                if (camundaType.AllowedIn.Any())
                {
                    file.WriteLine($"\t\t// Allowed: {string.Join(", ", camundaType.AllowedIn)}");
                }
                file.WriteLine();
                
                if (camundaType.Attributes.Any())
                {
                    file.WriteLine("\t\t// Attributes:");
                    foreach (var (attName, (attType, attDefault)) in camundaType.Attributes)
                    {
                        file.Write($"\t\tpublic {ConvertCSName(attType)}? {CapitalizeFirstLetter(attName)}");
                        file.Write(" {get; set;} ");
                        if (attDefault != null)
                        {
                            file.Write(" = "+ ConvertDefaultValue(attDefault)+ ";");
                        }
                        file.WriteLine();
                    }
                    file.WriteLine();
                }
                
                if (camundaType.ExtensionElements.Any())
                {
                    if (camundaType.Attributes.Any()) file.WriteLine();
                    file.WriteLine("\t\t// Extension Elements:");
                    foreach (var (elName, (elType, elIsBody, elIsMany)) in camundaType.ExtensionElements)
                    {
                        if (elIsBody)
                        {
                            file.Write($"\t\tpublic {ConvertCSName(elType)}? {CapitalizeFirstLetter(elName)}");
                            file.Write(" { get; set; } ");
                            file.WriteLine();
                        }
                        else if (elIsMany)
                        {
                            file.Write($"\t\tpublic List<{ConvertCSName(ConvertName(elType))}> {CapitalizeFirstLetter(elName)}");
                            file.Write(" { get; } = new();");
                            file.WriteLine();
                        }
                        else
                        {
                            file.Write($"\t\tpublic {ConvertCSName(ConvertName(elType))}? {CapitalizeFirstLetter(elName)}");
                            file.Write(" { get; set; } ");
                            file.WriteLine();
                        }
                    }
                }
                
                file.WriteLine("\t}\n");
            }


            file.WriteLine("}");
        }

        private static string ConvertDefaultValue(string defaultValue)
        {
            if (defaultValue == "False") return "false";

            throw new BPMNCheckerExceptions("Something went wrong, there is unsupported default camunda value: " + defaultValue);
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

        private static string CapitalizeFirstLetter(string name)
        {
            if (name != null && name.Length > 0 && char.IsLower(name[0]))
            {
                return char.ToUpper(name[0]) + name.Substring(1);
            }
            return string.Empty;
        }

        private static string ConvertName(string name)
        {
            if (name == "Element" || name == "String" || name == "Integer" || name == "Boolean") return name;
            if (name.StartsWith("bpmn:") || name.StartsWith("camunda:")) return name;
            return "camunda:" + name;
        }

        public static void EnrichGenerator(ILogger logger, Generator generator, bool generateCamundaClasses = false, string? toPythonFileName = null)
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

            if (generateCamundaClasses) GenerateCamundaClasses(camundaTypes);

            foreach (var (name, camundaType) in camundaTypes)
            {
                camundaType.Dump(dump);
                var allInInheritance = camundaType.GetAllInInheritance(camundaTypes);
                /*
                if (allInInheritance.Count > 1)
                {
                    dump.Write(">>> ");
                    dump.WriteLine(string.Join(", ", allInInheritance));
                }
                */
                var attributes = new List<(string Name, string Type, string? Default)>();
                var extensionElements = new List<(string Name, string Type, bool IsBody, bool IsMany)>();

                var allExtendingIt = new HashSet<string>() { name };
                // camunda types are sometime extending other camunda types
                int allExtendingItLength = allExtendingIt.Count;
                do
                {
                    allExtendingItLength = allExtendingIt.Count;

                    foreach(var currentName in allExtendingIt.ToHashSet())
                    {
                        var currentExtends = camundaTypes.Values.Where(x => x.Extends.Contains(currentName)).Select(x => x.Name);
                        foreach(var currentExtendingIt in currentExtends)
                        {
                            allExtendingIt.Add(currentExtendingIt);
                        }
                    }

                } while (allExtendingItLength < allExtendingIt.Count);

               
                foreach (var typeName in allInInheritance.Union(allExtendingIt))
                {
                    if (typeName.StartsWith("camunda"))
                    {
                        attributes.AddRange(camundaTypes[typeName].Attributes.Select(x=> (x.Key, x.Value.Type, x.Value.Default)));
                        extensionElements.AddRange(camundaTypes[typeName].ExtensionElements.Select(x=>(x.Key, x.Value.Type, x.Value.IsBody, x.Value.IsMany)));
                    }else
                    {
                        //Assuming there is just one type: bpmn:ErrorEventDefinition
                        if (!typeName.Equals("bpmn:ErrorEventDefinition"))
                        {
                            throw new BPMNCheckerExceptions("In camunda.json, Camunda elements are extending only one BPMN element ErrorEventDefinition.");
                        }
                        
                        attributes.Add(("id", "String", null));
                        attributes.Add(("errorRef", "Error", null));
                    }
                }

                if (camundaType.Extends.Any())
                {
                    
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

                //if (camundaType.IsAbstract==false)
                {

                    dump.WriteLine($"{camundaType.Name}");
                    dump.WriteLine($"Inheritance :  {string.Join(", ", allInInheritance)}");
                    dump.WriteLine($"Extended by :  {string.Join(", ", allExtendingIt)}");
                    dump.WriteLine($"Allowed in  : {string.Join(",", camundaType.AllowedIn)}");
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

            dump.WriteLine($"Processed: {camundaTypes.Count(x => x.Value.IsAbstract)} abstract types extending BPMN types and {camundaTypes.Count(x => !x.Value.IsAbstract)} Camunda types.");

            if (toPythonFileName != null)
            {
                using var toPython = new StreamWriter(toPythonFileName);

                toPython.WriteLine("extracted = {");

                foreach (var item in generator.Types)
                {
                    if (item.Value is ComplexType c)
                    {
                        (List<BPMNModel.Attribute> Attributes, List<string> Elements) GetAll(ComplexType current)
                        {
                            List<BPMNModel.Attribute> attributes = new();
                            List<string> elements = new();
                            if (current.ParentType is not null)
                            {
                                var parent = GetAll(current.ParentType);
                                attributes.AddRange(parent.Attributes);
                                elements.AddRange(parent.Elements);
                            }
                            attributes.AddRange(current.Attributes);
                            var currentElements = current.InnerElement?.InnerElements.Select(x => $"\"{x.Name}\"").ToList();

                            if (currentElements is not null && currentElements.Any()) elements.AddRange(currentElements);

                            return (attributes, elements);
                        }
                        var (allAttributes, allElements) = GetAll(c);

                        var required = allAttributes.Where(x => x.Use == AttributeUse.Required).Select(x => $"\"{x.Name}\"").ToList();
                        var optional = allAttributes.Where(x => x.Use == AttributeUse.Optional).Select(x => $"\"{x.Name}\"").ToList();
                        toPython.WriteLine($"\t\"{c.Name.Substring(1)}\": ([{string.Join(", ", required)}], [{string.Join(", ", optional)}], [{string.Join(", ", allElements ?? [])}]),");
                    }
                }
                toPython.WriteLine("\t}\n");


                toPython.WriteLine("camundaAttributes = {");
                foreach (var (bpmnType, attributes)  in bpmnCamundaAttributes)
                {
                    toPython.Write($"  \"{bpmnType.Substring(1)}\" : [");
                    toPython.Write(string.Join(", ", attributes.Select(x => $"(\"{x.Type}\",\"{x.Name}\")")));
                    toPython.WriteLine("],");
                }
                toPython.WriteLine("}");
            }
        }
    }
}
