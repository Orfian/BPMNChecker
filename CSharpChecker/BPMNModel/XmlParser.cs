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
        private ILogger logger;

        private Generator generator;

        public XmlParserNode? Root { get; private set; }

        public List<string> Errors { get; } = new List<string>();

        public bool HasErrors { get => Errors.Any(); }

        private XNamespace bpmnNamespace;

        private XNamespace xsiNamespace;

        private XmlParser(ILogger logger, Generator generator, XNamespace bpmnNamespace, XNamespace xsiNamespace)
        {
            this.logger = logger;
            this.generator = generator;
            this.bpmnNamespace = bpmnNamespace;
            this.xsiNamespace = xsiNamespace;
        }

        
        private string GetNiceNamespace(XNamespace @namespace)
        {
            if (@namespace == bpmnNamespace) return "bpmn:";
            if (@namespace == xsiNamespace) return "xsi:";
            return @namespace.NamespaceName;
        }
        private string GetNiceName(XName name)
        {
            return GetNiceNamespace(name.NamespaceName)+name.LocalName;
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


        private XmlParserNode? LoadAndCheck(XElement element, RootElement type)
        {
            logger.Debug(GetNiceMessage(element, $"Starting to load {GetNiceName(element.Name)}"));

            if (element.Name.LocalName != type.Name)
            {
                Errors.Add(GetNiceMessage(element, $"Expecting element {type.Name}."));
                return null;
            }

            if (type.Type.HasMixedContent)
            {
                //TODO: Processing mixed content.
            }

            var processedAttributes = new List<XmlParserAttribute>();
            var allAttributes = type.Type.GetAllAttributes();

            Log.Debug(GetNiceMessage(element, $"  All attributes: "));
            foreach (var attribute in allAttributes)
            {
                Log.Debug(GetNiceMessage(element, $"    {attribute}"));
            }

            foreach (var attribute in allAttributes)
            {
                var (xmlAttribute, message) = attribute.CreateAndCheck(element);
                if (xmlAttribute != null)
                {
                    Log.Debug(GetNiceMessage(element, $"  Created attribute: {xmlAttribute}"));
                    processedAttributes.Add(xmlAttribute);
                }
                if (message != null)
                {
                    Errors.Add(GetNiceMessage(element, message));
                }
            }

            var idAttributes = processedAttributes.Where(x => x.Name == "id");

            if (idAttributes.Count() > 1)
            {
                Errors.Add(GetNiceMessage(element, $"element should have at most one id."));
                return null;
            }

            XmlParserComplexNode node = idAttributes.Any() ? XmlParserComplexNode.CreateOrGet(idAttributes.First().Value, type.Type) : XmlParserComplexNode.CreatePlaceholderForAnyNodes();
            node.Attributes.AddRange(processedAttributes);


            if (type.InnerComplexType is not null)
            {
                var (allCategories, categoriesRestrictions) = type.InnerComplexType.GetAllInformations();
                int currentCategoryIndex = 0;

                Log.Debug(GetNiceMessage(element, $"  All elements: "));
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

                                    var castAttribute = currentElement.Attribute(xsiNamespace + "type");
                                    if  (castAttribute is not null)
                                    {
                                        var castType = castAttribute.Value;

                                        if (castType.StartsWith("bpmn:")) {
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
                                    var targetNode = XmlParserComplexNode.GetReferecne(realValue);
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
                    int minOccurences = currentCategory.MinOccurs??0;
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
                    foreach (var (A,B) in categoriesRestrictions)
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

            logger.Debug(GetNiceMessage(element, $"Finished loading {GetNiceName(element.Name)}: id={node.ID}, type={node.Type?.Name}, attributes: {node.Attributes.Count}, categories: {node.ChildNodes.Count} with {node.ChildNodes.Values.Select(x=>x.Count).Sum()} elements."));
            return node;
        }


        public static XmlParser Parse(ILogger logger, Generator generator, XDocument document)
        {
            if (document.Root == null) throw new BPMNCheckerExceptions($"File has no root element.");

            //TODO: Removing diagram element from definition.
            var diagrams = document.Root.Elements().Where(e => e.Name.Namespace == "http://www.omg.org/spec/BPMN/20100524/DI");
            foreach (var diagramElement in diagrams)
            {
                diagramElement.Remove();
            }

            var bpmnNamespaceString = document.Root.Attribute(XNamespace.Xmlns+"bpmn")?.Value;
            var xsiNamespaceString = document.Root.Attribute(XNamespace.Xmlns+"xsi")?.Value;

            if (string.IsNullOrEmpty(bpmnNamespaceString) || string.IsNullOrEmpty(xsiNamespaceString))
            {
                throw new BPMNCheckerExceptions($"There are no required namespaces attributes: {"xmlns:bpmn"} and {"xmlns:xsi"}");
            }

            XNamespace bpmnNamespace = XNamespace.Get(bpmnNamespaceString);
            XNamespace xsiNamespace = XNamespace.Get(xsiNamespaceString);


            XmlParser parser = new XmlParser(logger, generator, bpmnNamespace, xsiNamespace);
            parser.Root = parser.LoadAndCheck(document.Root, generator.Elements["definitions"]);

            var error = XmlParserComplexNode.CheckReferencesWithoutDefinitions();
            if (error is not null) parser.Errors.Add(error);

            return parser;
        }
    }
}
