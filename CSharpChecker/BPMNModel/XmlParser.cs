using BPMNModel.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public class XmlParser
    {
        private Generator generator;
        private ILogger logger;
        public Dictionary<string, XNamespace> namespaces;
        private XmlParser(ILogger logger, Generator generator, Dictionary<string, XNamespace> namespaces)
        {
            this.logger = logger;
            this.generator = generator;
            this.namespaces = namespaces;
        }

        public Dictionary<string, XmlParserComplexNode> Cache { get; } = new();

        public List<string> Errors { get; } = new List<string>();
        public bool HasErrors { get => Errors.Any(); }
        public XmlParserComplexNode? Root { get; private set; }
        public static XmlParser Parse(ILogger logger, Generator generator, XDocument document)
        {
            if (document.Root == null) throw new BPMNCheckerExceptions($"File has no root element.");

            //TODO: Removing diagram element from definition.
            var diagrams = document.Root.Elements().Where(e => e.Name.Namespace == "http://www.omg.org/spec/BPMN/20100524/DI");
            foreach (var diagramElement in diagrams)
            {
                diagramElement.Remove();
            }
            var namespaces = new Dictionary<string, XNamespace>
            {
                { "bpmn", XNamespace.Get("http://www.omg.org/spec/BPMN/20100524/MODEL") },
                { "xsi", XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") },
                { "camunda", XNamespace.Get("http://camunda.org/schema/1.0/bpmn") }
            };

            var namespacesInXml = document.Root.Attributes().Where(x=>x.Name.NamespaceName == XNamespace.Xmlns);

            foreach(var xmlnsAttribute in namespacesInXml)
            {
                namespaces[xmlnsAttribute.Name.LocalName] = xmlnsAttribute.Value;
            }

            XmlParser parser = new XmlParser(logger, generator, namespaces);
            parser.Root = parser.LoadAndCheck(document.Root, generator.Elements["definitions"]);

            var error = parser.CheckReferencesWithoutDefinitions();
            if (error is not null) parser.Errors.Add(error);

            return parser;
        }

        public string? CheckReferencesWithoutDefinitions()
        {
            var justReference = Cache.Where(x => x.Value.Type is null).Select(x => x.Key);

            if (justReference.Any())
            {
                return $"IDs: {string.Join(",", justReference)} were referenced but not defined.";
            }
            return null;
        }

        public XmlParserComplexNode CreateOrGet(string id, ComplexType complexType)
        {
            if (Cache.ContainsKey(id))
            {
                var cached = Cache[id];
                cached.Type = complexType;
                return cached;
            }
            else
            {
                var cached = new XmlParserComplexNode(id, complexType);
                Cache.Add(id, cached);
                return cached;
            }
        }

        public XmlParserComplexNode GetReferecne(string id)
        {
            if (Cache.ContainsKey(id))
            {
                var cached = Cache[id];
                return cached;
            }
            else
            {
                var cached = new XmlParserComplexNode(id);
                Cache.Add(id, cached);
                return cached;
            }
        }

        public void DumpXML(string fileName)
        {
            using var writer = new StreamWriter(fileName);

            foreach (var (_,item) in Cache)
            {
                item.DumpNode(writer, "");
            }
        }

        public XName GetXName(string name)
        {
            if (name.Contains(':'))
            {
                var namespaceString = name.Substring(0, name.IndexOf(':'));
                var localName = name.Substring(name.IndexOf(":")+1);
                if (namespaces.ContainsKey(namespaceString))
                {
                    return namespaces[namespaceString] + localName;
                }else
                {
                    throw new BPMNCheckerExceptions($"In the model, there should be no unknown namespacesl like: {namespaceString}.");
                }
            }
            else
            {
                return name;
            }
        }

        private string GetNiceMessage(XElement element, string text)
        {
            if (element is IXmlLineInfo info && info.HasLineInfo())
            {
                return $"{GetNiceName(element.Name)}[{info.LineNumber}:{info.LinePosition}] - {text}";
            }
            else
            {
                return $"{GetNiceName(element.Name)}[---] - {text}";
            }
        }

        private string GetNiceName(XName name)
        {
            return GetNiceNamespace(name.NamespaceName) + name.LocalName;
        }

        private string GetNiceNamespace(XNamespace @namespace)
        {
            if (namespaces.Values.Contains(@namespace))
            {
                var item = namespaces.Where(x => x.Value == @namespace).First();
                return item.Key;
            }
             return @namespace.NamespaceName;
        }

        private XmlParserComplexNode? LoadAndCheck(XElement element, RootElement type)
        {
            logger.Debug(GetNiceMessage(element, $"Starting to load {GetNiceName(element.Name)}"));

            if (element.Name.LocalName != type.Name)
            {
                Errors.Add(GetNiceMessage(element, $"Expecting element {type.Name}."));
                return null;
            }
           
            var processedAttributes = new List<XmlParserAttribute>();
            var allAttributes = type.Type.GetAllAttributes();

            Log.Debug(GetNiceMessage(element, $"  All attributes: "));
            foreach (var attribute in allAttributes)
            {
                Log.Debug(GetNiceMessage(element, $"    {attribute}"));
            }

            var allXMLAttributes = element.Attributes().Select(x=>x.Name).ToList();

            foreach (var attribute in allAttributes)
            {
                var (xmlAttribute, message) = attribute.CreateAndCheck(element, this);
                if (xmlAttribute != null)
                {
                    var resultOfRemoval = allXMLAttributes.Remove(xmlAttribute.XmlName);
                    
                    Log.Debug(GetNiceMessage(element, $"  Created attribute: {xmlAttribute}"));
                    processedAttributes.Add(xmlAttribute);
                }
                if (message != null)
                {
                    Errors.Add(GetNiceMessage(element, message));
                }
            }

            foreach (var xmlAttribute in allXMLAttributes.Where(x => x.NamespaceName != XNamespace.Xmlns))
            {
                Errors.Add(GetNiceMessage(element, $"Attribute {xmlAttribute} is not processed."));
            }

            var idAttributes = processedAttributes.Where(x => x.Name == "id");

            if (idAttributes.Count() > 1)
            {
                Errors.Add(GetNiceMessage(element, $"element should have at most one id."));
                return null;
            }

            XmlParserComplexNode node = idAttributes.Any() ? CreateOrGet(idAttributes.First().Value, type.Type) : XmlParserComplexNode.CreatePlaceholderForAnyNodes(type.Type);

            if (type.Type.HasMixedContent)
            {
                //TODO: Processing mixed content right?
                node.MixedContent = element.Value;
            }

            foreach (var item in processedAttributes)
            {
                node.Attributes.Add(item.Name, item);
            }

            if (type.InnerComplexType is not null)
            {
                var (allCategories, categoriesRestrictions) = type.InnerComplexType.GetAllInformations();
                int currentCategoryIndex = 0;

                logger.Debug(GetNiceMessage(element, $"  All elements: "));
                foreach (var item in allCategories)
                {
                    logger.Debug(GetNiceMessage(element, $"    {item}"));
                }

                var allElements = element.Elements().ToList();
                int currentElementIndex = 0;

                while (currentCategoryIndex < allCategories.Count)
                {
                    var currentCategory = allCategories[currentCategoryIndex];
                    node.ChildNodes.Add(currentCategory.Name, []);
                    currentCategoryIndex++;

                    logger.Debug(GetNiceMessage(element, $"  Processing: {currentCategory.Name}"));

                    while (currentElementIndex < allElements.Count())
                    {
                        var currentElement = allElements[currentElementIndex];
                        var localName = currentElement.Name.LocalName;

                        if (currentCategory is ReferenceElement reference)
                        {
                            if (generator.Elements.ContainsKey(localName))
                            {
                                var realElementCategory = generator.Elements[localName];

                                if (reference.ReferencedElement == realElementCategory ||
                                    (realElementCategory.Group is not null && realElementCategory.Group == reference.Name))
                                {
                                    var createdNode = LoadAndCheck(currentElement, generator.Elements[localName]);
                                    if (createdNode != null)
                                    {
                                        node.ChildNodes[currentCategory.Name].Add(createdNode);
                                        Log.Debug(GetNiceMessage(element, $"    {createdNode}"));
                                    }
                                    currentElementIndex++;
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        }
                        else if (currentCategory is NamedElement namedElement)
                        {
                            if (namedElement.Name == localName)
                            {
                                if (namedElement.Category == ElementXMLType.ComplexType)
                                {
                                    if (namedElement.InnerComplexType is null)
                                    {
                                        throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({namedElement.Name} refers to nonexisting complex type).");
                                    }

                                    var castAttribute = currentElement.Attribute(namespaces["xsi"] + "type");
                                    if (castAttribute is not null)
                                    {
                                        var castType = castAttribute.Value;

                                        if (castType.StartsWith("bpmn:"))
                                        {
                                            castType = castType.Substring(5);
                                            if (generator.Types.ContainsKey(castType) && generator.Types[castType] is ComplexType castingType)
                                            {
                                                var createdNode = new XmlParserCastNode(castingType, currentElement.Value);
                                                node.ChildNodes[currentCategory.Name].Add(createdNode);
                                                Log.Debug(GetNiceMessage(element, $"    {createdNode}"));
                                            }
                                            else
                                            {
                                                Errors.Add(GetNiceMessage(element, $"{currentCategory.Name} there is referenced unknown type {castType}."));
                                            }

                                            currentElementIndex++;
                                            continue;
                                        }
                                        else
                                        {
                                            throw new NotImplementedException();
                                        }
                                    }

                                    /*
                                    var createdNode = LoadAndCheck(currentElement, namedElement.InnerComplexType);
                                    if (createdNode != null)
                                    {
                                        node.ChildNodes[currentCategory.Name].Add(createdNode);
                                    }*/

                                    throw new NotImplementedException();
                                }
                                else
                                {
                                    var realValue = currentElement.Value;
                                    var targetNode = GetReferecne(realValue);
                                    node.ChildNodes[currentCategory.Name].Add(targetNode);
                                    Log.Debug(GetNiceMessage(element, $"    {targetNode}"));
                                }
                                currentElementIndex++;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (currentCategory is AnyElement)
                        {
                            //TODO: Use namespace
                            var createdNode = new XmlParserAnyNode(currentElement);
                            node.ChildNodes[currentCategory.Name].Add(createdNode);
                            Log.Debug(GetNiceMessage(element, $"    {createdNode}"));
                            currentElementIndex++;
                            continue;
                        }
                        else
                        {
                            Errors.Add(GetNiceMessage(element, $"Unexpected category: {currentCategory.Name}"));
                            break;
                        }
                    }

                    int realOccurences = node.ChildNodes[currentCategory.Name].Count;
                    if (currentCategory.MinOccurs is null || currentCategory.MaxOccurs is null) throw new BPMNCheckerExceptions($"For {currentCategory.Name} something went wrong - number of occurences should be filed now.");
                    int minOccurences = currentCategory.MinOccurs ?? 0;
                    int maxOccurences = currentCategory.MaxOccurs ?? 0;
                    logger.Debug(GetNiceMessage(element, $"  Checking:   {currentCategory.Name} - occurences: min={minOccurences} max={maxOccurences} real={realOccurences}"));

                    if (realOccurences < minOccurences) Errors.Add(GetNiceMessage(element, $"{currentCategory.Name} requires minimum {minOccurences} occurences, but there are {realOccurences}."));
                    if (realOccurences > maxOccurences) Errors.Add(GetNiceMessage(element, $"{currentCategory.Name} requires maximum {maxOccurences} occurences, but there are {realOccurences}."));
                }

                if (categoriesRestrictions.Any())
                {
                    Log.Debug(GetNiceMessage(element, $"  Restrictions:"));
                    foreach (var item in categoriesRestrictions)
                    {
                        Log.Debug(GetNiceMessage(element, $"    {item.A} vs {item.B}"));
                    }
                    foreach (var (A, B) in categoriesRestrictions)
                    {
                        if (node.ChildNodes.ContainsKey(A) && node.ChildNodes.ContainsKey(B))
                        {
                            int aOccurences = node.ChildNodes[A].Count;
                            int bOccurences = node.ChildNodes[B].Count;

                            if (aOccurences > 0 && bOccurences > 0)
                            {
                                Errors.Add(GetNiceMessage(element, $"Elements {A} and {B} can not be used at the same time."));
                            }
                        }
                    }
                }
            }

            logger.Debug(GetNiceMessage(element, $"Finished loading {GetNiceName(element.Name)}: id={node.ID}, type={node.Type?.Name}, attributes: {node.Attributes.Count}, categories: {node.ChildNodes.Count} with {node.ChildNodes.Values.Select(x => x.Count).Sum()} elements."));
            return node;
        }
    }
}