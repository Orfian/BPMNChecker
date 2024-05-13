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

        private XmlParser(ILogger logger, Generator generator)
        {
            this.logger = logger;
            this.generator = generator;
        }

        private static string GetNiceMessage(XElement element, string text)
        {
            if (element is IXmlLineInfo info && info.HasLineInfo())
            {
                return $"{element.Name.LocalName}[{info.LineNumber}:{info.LinePosition}] - {text}";
            }
            else
            {
                return $"{element.Name.LocalName}[---] - {text}";
            }
        }


        private XmlParserNode? LoadAndCheck(XElement element, RootElement type)
        {
            logger.Debug(GetNiceMessage(element, $"Starting to load {element.Name.LocalName}"));

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

            var idAttribute = processedAttributes.First(x => x.Name == "id");

            if (idAttribute == null)
            {
                Errors.Add(GetNiceMessage(element, $"every element should have id."));
                return null;
            }

            XmlParserNode node = XmlParserNode.CreateOrGet(idAttribute.Value, type.Type);
            node.Attributes.AddRange(processedAttributes);


            if (type.InnerComplexType is not null)
            {
                var allCategories = type.InnerComplexType.GetAllElements().ToList();
                int currentCategoryIndex = 0;

                Log.Debug(GetNiceMessage(element, $"  All elements: "));
                foreach (var item in allCategories)
                {
                    Log.Debug(GetNiceMessage(element, $"    {item}"));
                }

                
                var allElements = element.Elements().ToList();
                int currentElementIndex = 0;

                while (currentCategoryIndex < allCategories.Count)
                {

                    var currentCategory = allCategories[currentCategoryIndex];
                    node.ChildNodes.Add(currentCategory.Name, []);
                    currentCategoryIndex++;

                    Log.Debug(GetNiceMessage(element, $"  Processing: {currentCategory.Name}"));

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
                                    /*
                                    var createdNode = LoadAndCheck(currentElement, namedElement.InnerComplexType);
                                    if (createdNode != null)
                                    {
                                        node.ChildNodes[currentCategory.Name].Add(createdNode);
                                    }*/
                                }
                                else
                                {
                                    var realValue = currentElement.Value;
                                    var targetNode = XmlParserNode.GetReferecne(realValue);
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
                        else
                        {
                            Errors.Add(GetNiceMessage(element, $"Unexpected category: {currentCategory.Name}"));
                        }
                    }
                }
            }

            logger.Debug(GetNiceMessage(element, $"Finished loading {element.Name.LocalName}: id={node.ID}, type={node.Type?.Name}, attributes: {node.Attributes.Count}, categories: {node.ChildNodes.Count} with {node.ChildNodes.Values.Select(x=>x.Count).Sum()} elements."));
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
            XmlParser parser = new XmlParser(logger, generator);
            parser.Root = parser.LoadAndCheck(document.Root, generator.Elements["definitions"]);

            var error = XmlParserNode.CheckReferencesWithoutDefinitions();
            if (error is not null) parser.Errors.Add(error);

            return parser;
        }
    }
}
