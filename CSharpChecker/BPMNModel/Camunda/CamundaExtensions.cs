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
        public static void EnrichGenerator(ILogger logger, Generator generator)
        {
            string ConvertName(string name)
            {
                if (name.StartsWith("bpmn:") || name.StartsWith("camunda:")) return name;
                return "camunda:" + name;
            }




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
                    dump.WriteLine(camundaType.Name + " "+ camundaType.IsAbstract);
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
                            }
                        }else
                        {
                            throw new BPMNCheckerExceptions($"In camunda.json, {camundaType.Name} is extending wrong type in BMPN.");
                        }

                        /*
                        dump.WriteLine("  "+extension);
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
                        */
                    }
                }
               
            }
        }
    }
}
