using BPMNModel.XMLElements;
using Serilog;
using System.Text.Json;
using Utility;
using Attribute = BPMNModel.XMLElements.Attribute;

namespace BPMNModel.Camunda
{
    public class CamundaExtensions
    {
        public static string ConvertCSName(string name)
        {
            if (name == "Element") return name;
            if (name == "String") return "string";
            if (name == "Integer") return "long";
            if (name == "Boolean") return "bool";
            if (name.StartsWith("bpmn")) return name.Replace("bpmn:", "");
            //camunda name
            return name.Replace("camunda:", "Camunda");
        }

        public static void EnrichGenerator(ILogger logger, Generator generator, bool generateCamundaClasses = false, string? toPythonFileName = null)
        {
            var camundaJsonTypes = new Dictionary<string, CamundaJSonType>();

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

                var camundaJsonType = new CamundaJSonType(ConvertName(typeName), isAbstract, superClass);

                if (typeJSON.TryGetProperty("extends", out JsonElement extendsProperty))
                {
                    camundaJsonType.Extends.AddRange(extendsProperty.EnumerateArray().Select(x => x.GetString()).Where(x => x != null).Cast<string>());
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

                            camundaJsonType.Attributes.Add(attName, (attType, attDefault));

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

                            camundaJsonType.ExtensionElements.Add(attName, (attType, attIsBody, attIsMany));
                        }
                    }
                }

                if (typeJSON.TryGetProperty("meta", out JsonElement metaProperty))
                {
                    if (metaProperty.TryGetProperty("allowedIn", out JsonElement allowedProperty))
                    {
                        camundaJsonType.AllowedIn.AddRange(allowedProperty.EnumerateArray().Select(x => x.GetString()).Where(x => x != null).Cast<string>());
                    }
                }

                camundaJsonTypes.Add(camundaJsonType.Name, camundaJsonType);
            }
            using var dump = new StreamWriter("camundaProcessed");

            var bpmnCamundaAttributes = new Dictionary<string, List<(string Type, string Name)>>();

            var uniqueTypesList = new SortedSet<string>();

            if (generateCamundaClasses) GenerateCamundaClasses(camundaJsonTypes);

            var allCamundaTypes = new Dictionary<string, CamundaElementType>();

            foreach (var (name, camundaJsonType) in camundaJsonTypes)
            {
                //camundaType.Dump(dump);
                var allInInheritance = camundaJsonType.GetAllInInheritance(camundaJsonTypes);
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

                    foreach (var currentName in allExtendingIt.ToHashSet())
                    {
                        var currentExtends = camundaJsonTypes.Values.Where(x => x.Extends.Contains(currentName)).Select(x => x.Name);
                        foreach (var currentExtendingIt in currentExtends)
                        {
                            allExtendingIt.Add(currentExtendingIt);
                        }
                    }
                } while (allExtendingItLength < allExtendingIt.Count);

                foreach (var typeName in allInInheritance.Union(allExtendingIt))
                {
                    if (typeName.StartsWith("camunda"))
                    {
                        attributes.AddRange(camundaJsonTypes[typeName].Attributes.Select(x => (x.Key, x.Value.Type, x.Value.Default)));
                        extensionElements.AddRange(camundaJsonTypes[typeName].ExtensionElements.Select(x => (x.Key, x.Value.Type, x.Value.IsBody, x.Value.IsMany)));
                    }
                    else
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

                if (camundaJsonType.Extends.Any())
                {
                    foreach (var extension in camundaJsonType.Extends.Where(x => !x.StartsWith("camunda")))
                    {
                        if (extensionElements.Any())
                        {
                            throw new BPMNCheckerExceptions($"In camunda.json, all Camunda types extending BPMN types assumes to have just attributes ({camundaJsonType.Name} have Extension Elements).");
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
                                    _ => throw new BPMNCheckerExceptions($"In camunda.json, unexpected attribute: {attType} in {camundaJsonType.Name}")
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
                        }
                        else
                        {
                            throw new BPMNCheckerExceptions($"In camunda.json, {camundaJsonType.Name} is extending wrong type in BMPN.");
                        }
                    }
                }

                if (camundaJsonType.AllowedIn.Any())
                {
                    foreach (var allowedIn in camundaJsonType.AllowedIn)
                    {
                        if (allowedIn.StartsWith("bpmn") || allowedIn.Equals("*"))
                        {
                            var xmlTypeName = allowedIn.Equals("*") ? "tBaseElement" : allowedIn.Replace("bpmn:", "t");
                            var xmlType = generator.Types[xmlTypeName];

                            if (xmlType is ComplexType complexType)
                            {
                                complexType.AllowedCamundaElements.Add(camundaJsonType);
                            }
                            else
                            {
                                throw new BPMNCheckerExceptions($"In camunda.json, {camundaJsonType.Name} is extending wrong type in BMPN.");
                            }
                        }
                        else
                        {
                            //what remains are only camunda elements in AllowedIn, adding BPMN elements listed in extending section of the target, no more inheritance present.
                            //TODO: camunda:InputOutput in allowed in camunda:Connector, that has no effect right?
                            var extendingCamundaType = camundaJsonTypes[allowedIn];
                            foreach (var extendedBPMNType in extendingCamundaType.Extends)
                            {
                                var xmlTypeName = extendedBPMNType.Replace("bpmn:", "t");
                                var xmlType = generator.Types[xmlTypeName];

                                if (xmlType is ComplexType complexType)
                                {
                                    complexType.AllowedCamundaElements.Add(camundaJsonType);
                                }
                                else
                                {
                                    throw new BPMNCheckerExceptions($"In camunda.json, {extendingCamundaType.Name} is extending wrong type in BMPN.");
                                }
                            }
                        }
                    }
                }

                camundaJsonType.AllExtensionElements.AddRange(extensionElements);

                var camundaElementType = new CamundaElementType(camundaJsonType);

                foreach (var attribute in attributes)
                {
                    AttributeXMLType xmlAttType = attribute.Type switch
                    {
                        "String" => AttributeXMLType.String,
                        "Boolean" => AttributeXMLType.Boolean,
                        "Integer" => AttributeXMLType.Integer,
                        //For camunda:ErrorEventDefinition, it is refering  bpmn:ErrorEventDefinition
                        "Error" => AttributeXMLType.IDRef,
                        _ => throw new BPMNCheckerExceptions($"In camunda.json, unknown attribute type {attribute.Type}."),
                    };
                    var processedAttribute = new Attribute(attribute.Name, (xmlAttType, null));
                    if (attribute.Default != null)
                    {
                        processedAttribute.Default = attribute.Default;
                    }
                    camundaElementType.Attributes.Add(processedAttribute);
                }

                allCamundaTypes.Add(camundaJsonType.Name, camundaElementType);

                if (camundaJsonType.IsAbstract == false)
                {
                    //for next processing using only non-abstract types
                    camundaJsonType.Dump(dump);

                    generator.CamundaTypes.Add(ConvertTypeNameToElementName(camundaElementType.CamundaJSonType.Name), camundaElementType);

                    dump.WriteLine($"{camundaJsonType.Name}");
                    dump.WriteLine($"Inheritance :  {string.Join(", ", allInInheritance)}");
                    dump.WriteLine($"Extended by :  {string.Join(", ", allExtendingIt)}");
                    dump.WriteLine($"Allowed in  : {string.Join(",", camundaJsonType.AllowedIn)}");
                    dump.WriteLine("  All Processed Attributes:");
                    foreach (var attribute in camundaElementType.Attributes)
                    {
                        dump.WriteLine($"    {attribute}");
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

            foreach (var (elementName, camundaType) in allCamundaTypes)
            {
                if (camundaType.CamundaJSonType.SuperClass is not null)
                {
                    if (camundaType.CamundaJSonType.SuperClass.StartsWith("bpmn:"))
                    {
                        //TODO: Solving bpmn:ErrorEventDefinition in inheritance. Do I need to do anything?
                    }
                    else
                    {
                        var superCamundaType = allCamundaTypes[camundaType.CamundaJSonType.SuperClass];
                        camundaType.Parent = superCamundaType;
                    }
                }

                foreach (var (elName, elType, elIsBody, elIsMany) in camundaType.CamundaJSonType.AllExtensionElements)
                {
                    //dump.WriteLine($"    {elType} {elName}{(elIsBody ? " (Body)" : "")}{(elIsMany ? " (Many)" : "")}");
                    if (elIsBody)
                    {
                        if (elType != "String") throw new BPMNCheckerExceptions($"In camunda.json, element with attribute Body=true should be String, but it is {elType}.");
                        camundaType.BodyElementName = elName;
                    }
                    else
                    {
                        var convertedElementName = ConvertName(elName);

                        if (elType.StartsWith("bpmn:"))
                        {
                            var bpmnName = LowercaseFirstLetter(elType.Replace("bpmn:", ""));
                            if (generator.Elements.ContainsKey(bpmnName))
                            {
                                camundaType.InnerElementsByTypeElementName.Add(convertedElementName, generator.Elements[bpmnName]);
                            }else
                            {
                                throw new BPMNCheckerExceptions($"Unknown BPMN element in camunda.json: {elType}");
                            }
                        }
                        else if (elType == "String")
                        {
                            if (elIsMany) throw new BPMNCheckerExceptions($"In camunda.json, element of type String can not have attribute Many=true.");
                            camundaType.InnerElementsByTypeElementName.Add(convertedElementName, new CamundaValueElement(convertedElementName));
                        }
                        else
                        {
                            var camundaTypeName = ConvertName(elType);
                            if (allCamundaTypes.ContainsKey(camundaTypeName))
                            {
                                var innerElementType = allCamundaTypes[camundaTypeName];
                                camundaType.InnerElementsByTypeElementName.Add(convertedElementName, new CamundaElement(ConvertName(elName), innerElementType, elIsMany));
                            }
                            else
                            {
                                throw new BPMNCheckerExceptions($"In camunda.json, Camunda type: {camundaType.CamundaJSonType.Name} reference unknown type: {elType}.");
                            }
                        }
                    }
                }
            }

            dump.WriteLine($"Processed: {camundaJsonTypes.Count(x => x.Value.IsAbstract)} abstract types extending BPMN types and {camundaJsonTypes.Count(x => !x.Value.IsAbstract)} Camunda types.");

            foreach (var complexType in generator.Types.Values.Where(x => x is ComplexType).Cast<ComplexType>())
            {
                var allowedCamundaTypes = complexType.GetAllAllowedCamundaElements().Select(x => x.Name).ToList();
                dump.WriteLine($"{complexType.Name} - {string.Join(", ", allowedCamundaTypes)}");
            }

            if (toPythonFileName != null)
            {
                using var toPython = new StreamWriter(toPythonFileName);

                toPython.WriteLine("extracted = {");

                foreach (var item in generator.Types)
                {
                    if (item.Value is ComplexType c)
                    {
                        (List<XMLElements.Attribute> Attributes, List<string> Elements) GetAll(ComplexType current)
                        {
                            List<XMLElements.Attribute> attributes = new();
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
                        toPython.WriteLine($"\t\"{(c.Name.StartsWith("t") ? c.Name.Substring(1) : c.Name)}\": ([{string.Join(", ", required)}], [{string.Join(", ", optional)}], [{string.Join(", ", allElements ?? [])}]),");
                    }
                }
                toPython.WriteLine("\t}\n");

                toPython.WriteLine("camundaAttributes = {");
                foreach (var (bpmnType, attributes) in bpmnCamundaAttributes)
                {
                    toPython.Write($"  \"{bpmnType.Substring(1)}\" : [");
                    toPython.Write(string.Join(", ", attributes.Select(x => $"(\"{x.Type}\",\"{x.Name}\")")));
                    toPython.WriteLine("],");
                }
                toPython.WriteLine("}");
            }
        }

        private static string CapitalizeFirstLetter(string name)
        {
            if (name != null && name.Length > 0 && char.IsLower(name[0]))
            {
                return char.ToUpper(name[0]) + name.Substring(1);
            }
            return string.Empty;
        }

        private static string ConvertDefaultValue(string defaultValue)
        {
            if (defaultValue == "False") return "false";

            throw new BPMNCheckerExceptions("Something went wrong, there is unsupported default camunda value: " + defaultValue);
        }

        private static string ConvertName(string name)
        {
            if (name == "Element" || name == "String" || name == "Integer" || name == "Boolean") return name;
            if (name.StartsWith("bpmn:") || name.StartsWith("camunda:")) return name;
            return "camunda:" + name;
        }

        private static string ConvertTypeNameToElementName(string typeName)
        {
            if (typeName.StartsWith("camunda:"))
            {
                return "camunda:" + LowercaseFirstLetter(typeName.Replace("camunda:", ""));
            }
            throw new BPMNCheckerExceptions($"Converting to element name wrong Camunda name: {typeName}.");
        }

        private static void GenerateCamundaClasses(Dictionary<string, CamundaJSonType> data)
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

                    foreach (var (_, (fieldType, _)) in camundaType.Attributes)
                    {
                        var realFieldType = ConvertName(fieldType);

                        if (!typesToGenerate.Contains(realFieldType) &&
                            fieldType != "String" && fieldType != "Integer" && fieldType != "Boolean" && !fieldType.StartsWith("bpmn"))
                        {
                            typesToGenerate.Add(realFieldType);
                        }
                    }

                    foreach (var (_, (elementType, _, _)) in camundaType.ExtensionElements)
                    {
                        var realElementType = ConvertName(elementType);

                        if (!typesToGenerate.Contains(realElementType) &&
                            elementType != "String" && elementType != "Integer" && elementType != "Boolean" && !elementType.StartsWith("bpmn"))
                        {
                            typesToGenerate.Add(realElementType);
                        }
                    }
                }

                foreach (var (_, camundaType) in data.ToList())
                {
                    if (camundaType.SuperClass is not null && typesToGenerate.Contains(camundaType.SuperClass) && !typesToGenerate.Contains(camundaType.Name))
                    {
                        typesToGenerate.Add(camundaType.Name);
                    }
                    //Just checking, not realy adding anything.
                    foreach (var extendsName in camundaType.Extends)
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
            file.WriteLine($"{string.Join("\n", missing.Select(x => "// " + x.Key))}");
            file.WriteLine("// (Just extending attributes were generated for base types.)");
            file.WriteLine("// -------------------------------------------------------");

            file.WriteLine("using BPMNModel.Model;");
            file.WriteLine("using BPMNModel.XMLParser;");
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

                toExtend.Add("ICamundaLoaderBase");

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
                            file.Write(" = " + ConvertDefaultValue(attDefault) + ";");
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

                file.WriteLine($"\t\tpublic {(camundaType.SuperClass is null || camundaType.SuperClass.StartsWith("bpmn:") ? "" : "new")} void Load(XmlParserCamundaNode node)");
                file.WriteLine("\t\t{");

                if (camundaType.SuperClass is not null)
                {
                    if (camundaType.SuperClass.StartsWith("bpmn:"))
                    {
                        //TODO: Superclass is BPMN ErrorEventDefinition - do not have an example how to use it here - it is extended by camunda attributes i necessary.
                        file.WriteLine("\t\t\tthrow new Utility.BPMNCheckerExceptions(\"Do not know how to solve this - need to implement it in the generator.\");");
                    }
                    else
                    {
                        file.WriteLine("\t\t\tbase.Load(node);");
                    }
                }
                if (camundaType.SuperClass is null || !camundaType.SuperClass.StartsWith("bpmn:"))
                {
                    foreach (var (attName, (attType, attDefault)) in camundaType.Attributes)
                    {
                        file.WriteLine($"\t\t\t//Loading attribute: {attType} {attName}" + (attDefault is null ? "" : $"({attDefault})"));
                        if (attType.Equals("String"))
                        {
                            file.WriteLine($"\t\t\tif (node.Attributes.ContainsKey(\"{attName}\")) {CapitalizeFirstLetter(attName)} = node.Attributes[\"{attName}\"].Value;");
                            if (attDefault is not null)
                            {
                                file.WriteLine($"\t\t\telse {CapitalizeFirstLetter(attName)} = \"{attDefault}\";");
                            }
                        }
                        else if (attType.Equals("Boolean"))
                        {
                            file.WriteLine($"\t\t\tif (node.Attributes.ContainsKey(\"{attName}\")) {CapitalizeFirstLetter(attName)} = string.Equals(node.Attributes[\"{attName}\"].Value, \"true\");");
                            if (attDefault is not null)
                            {
                                file.WriteLine($"\t\t\telse {CapitalizeFirstLetter(attName)} = {(attDefault.Equals("true") ? "true" : "false")};");
                            }
                        }
                        else
                        {
                            throw new BPMNCheckerExceptions("Something went wrong, encountered unexpected attribute type: " + attType);
                        }
                        file.WriteLine();
                    }

                    foreach (var (elNameJSON, (elType, elIsBody, elIsMany)) in camundaType.ExtensionElements)
                    {
                        var convertedName = ConvertName(elNameJSON);
                        file.WriteLine($"\t\t\t//Loading element: {elType}{(elIsMany ? "*" : "")} {elNameJSON} - {convertedName}");

                        if (elIsBody)
                        {
                            file.WriteLine($"\t\t\tif (node.ChildNodes.ContainsKey(\"{elNameJSON}\") && node.ChildNodes[\"{elNameJSON}\"].Count==1) {CapitalizeFirstLetter(elNameJSON)} = ((XmlParserStringNode)node.ChildNodes[\"{elNameJSON}\"][0]).Value;");
                        }
                        else if (elType.Equals("String"))
                        {
                            if (elIsMany) throw new BPMNCheckerExceptions($"Something went wrong, for attribute {elNameJSON} with type String - processing: isMany = true is not implemented.");
                            file.WriteLine($"\t\t\tif (node.ChildNodes[\"{convertedName}\"].Count==1) {CapitalizeFirstLetter(elNameJSON)} = ((XmlParserStringNode)node.ChildNodes[\"{convertedName}\"][0]).Value;");
                        }
                        else if (elIsMany)
                        {
                            file.WriteLine($"\t\t\tif (node.ChildNodes[\"{convertedName}\"].Count>0) CamundaFactory.LoadElements<{ConvertCSName(ConvertName(elType))}>({CapitalizeFirstLetter(elNameJSON)}, node.ChildNodes[\"{convertedName}\"]);");
                        }
                        else
                        {
                            file.WriteLine($"\t\t\tif (node.ChildNodes[\"{convertedName}\"].Count==1) {CapitalizeFirstLetter(elNameJSON)} = ({ConvertCSName(ConvertName(elType))})CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes[\"{convertedName}\"][0]);");
                        }

                        file.WriteLine();
                    }
                }
                file.WriteLine("\t\t}");

                file.WriteLine("\t}\n");
            }

            file.WriteLine("}");
        }

        private static string LowercaseFirstLetter(string name)
        {
            return char.ToLower(name[0]) + name.Substring(1);
        }
    }
}