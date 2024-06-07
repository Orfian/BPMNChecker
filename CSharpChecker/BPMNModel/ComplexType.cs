using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public class ComplexType : ElementType
    {
        private XElement element;
        private ComplexType(string name, bool isAbstract, bool hasMixedContent, XElement element)
        {
            Name = name;
            IsAbstract = isAbstract;
            HasMixedContent = hasMixedContent;
            this.element = element;
        }

        public List<Attribute> Attributes { get; } = new();
        public bool HasMixedContent { get; init; }
        public ContainerElement? InnerElement { get; private set; }
        public bool IsAbstract { get; init; }
        public string Name { get; init; }
        public ComplexType? ParentType { get; private set; }
        public static ComplexType Create(XElement element)
        {
            var name = ElementType.GetExpectedAttribute(element, "name");

            bool isAbstract = ElementType.TryToGetBoolAttribute(element, "abstract");
            bool isMixed = ElementType.TryToGetBoolAttribute(element, "mixed");

            ComplexType result = new ComplexType(name, isAbstract, isMixed, element);

            return result;
        }

        public void FullLoad(XNamespace xs, Dictionary<string, ElementType> types, Dictionary<string, RootElement> elements)
        {
            ContainerElement LoadSequence(XElement innerElement)
            {
                var sequence = new ContainerElement();

                foreach (var sequenceElement in innerElement.Elements())
                {
                    if (sequenceElement.Name.LocalName != "element" && sequenceElement.Name.LocalName != "any")
                    {
                        throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (In {this.Name} there is a sequence with {sequenceElement.Name.LocalName}).");
                    }

                    sequence.InnerElements.Add(Element.Create(sequenceElement, elements, types));
                }

                return sequence;
            }

            void ProcessInnerElements(XElement start)
            {
                foreach (var innerElement in start.Elements())
                {
                    if (innerElement.Name.LocalName == "attribute")
                    {
                        DumpAttributesExcept(innerElement, ["name", "type", "use", "default"]);
                        DumpElementsExcept(innerElement, []);

                        var name = ElementType.GetExpectedAttribute(innerElement, "name");
                        var type = ElementType.GetExpectedAttribute(innerElement, "type");

                        var attribute = new Attribute(name, Attribute.LoadTypeFromString(type, types));

                        var useString = ElementType.TryToGetAttribute(innerElement, "use");
                        if (useString != null)
                        {
                            attribute.Use = useString switch
                            {
                                "optional" => AttributeUse.Optional,
                                "required" => AttributeUse.Required,
                                _ => throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (attribute use have unexpected value {useString}).")
                            };
                        }

                        attribute.Default = ElementType.TryToGetAttribute(innerElement, "default");

                        if (attribute.Use == AttributeUse.Required && attribute.Default is not null)
                        {
                            throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (attribute use and default can not be use together).");
                        }

                        Attributes.Add(attribute);
                    }
                    else if (innerElement.Name.LocalName == "sequence")
                    {
                        DumpElementsExcept(innerElement, ["element", "any"]);
                        DumpAttributesExcept(innerElement, []);

                        if (this.InnerElement != null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({this.Name} has more than one inner element - {innerElement.Name.LocalName}).");

                        var sequence = LoadSequence(innerElement);
                        this.InnerElement = sequence;
                    }
                    else if (innerElement.Name.LocalName == "choice")
                    {
                        DumpElementsExcept(innerElement, ["element", "sequence"]);
                        DumpAttributesExcept(innerElement, []);

                        if (this.InnerElement != null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({this.Name} has more than one inner element - {innerElement.Name.LocalName}).");

                        var choice = new ContainerElement();

                        foreach (var choiceElement in innerElement.Elements())
                        {
                            if (choiceElement.Name.LocalName != "element" && choiceElement.Name.LocalName != "sequence")
                            {
                                throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (In {this.Name} there is a sequence with {choiceElement.Name.LocalName}).");
                            }
                            if (choiceElement.Name.LocalName == "element")
                            {
                                choice.AddWithRestriction(Element.Create(choiceElement, elements, types));
                            }
                            if (choiceElement.Name.LocalName == "sequence")
                            {
                                choice.AddWithRestriction(LoadSequence(choiceElement));
                            }
                        }

                        this.InnerElement = choice;
                    }
                    else if (innerElement.Name.LocalName == "anyAttribute")
                    {
                        //TODO: Doing nothing...
                    }
                    else
                    {
                        throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({start.Name.LocalName} should not have element {innerElement.Name.LocalName}).");
                    }
                }
            }

            DumpAttributesExcept(element, ["name", "mixed", "abstract"]);
            if (element.Element(xs + "complexContent") is not null)
            {
                var complexContent = ElementType.GetExpectedSingleElement(element, "complexContent");
                DumpAttributesExcept(complexContent, Array.Empty<string>());
                var extension = ElementType.GetExpectedSingleElement(complexContent, "extension");
                var parent = ElementType.GetExpectedAttribute(extension, "base");
                DumpAttributesExcept(extension, ["base"]);

                var parentAsComplexType = types[parent] as ComplexType;
                this.ParentType = parentAsComplexType;
                ProcessInnerElements(extension);
            }
            else
            {
                ProcessInnerElements(element);
            }
        }

        public Attribute[] GetAllAttributes()
        {
            var result = new List<Attribute>();
            result.AddRange(this.Attributes);
            if (ParentType != null)
            {
                result.AddRange(ParentType.GetAllAttributes());
            }
            return result.ToArray();
        }

        public (List<Element> Categories, List<(string A, string B)> Restrictions) GetAllInformations()
        {
            var result = new List<Element>();
            var restrictions = new List<(string A, string B)>();

            if (ParentType != null)
            {
                var (parentElements, parentRestrictions) = ParentType.GetAllInformations();
                result.AddRange(parentElements);
                restrictions.AddRange(parentRestrictions);
            }

            if (InnerElement != null)
            {
                foreach (var item in InnerElement.InnerElements)
                {
                    if (item is Element element)
                    {
                        result.Add(element);
                    }
                    else
                    {
                        throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({this.Name} - there are only elements in a sequence).");
                    }
                }
                foreach (var restriction in InnerElement.Restrictions)
                {
                    restrictions.Add(restriction);
                }
            }

            return (result, restrictions);
        }
    }
}