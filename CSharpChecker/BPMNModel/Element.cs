using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public interface IElement
    {

    }

    public enum ElementXMLType
    {
        QName,
        IDRef,
        ComplexType,
    }

    public abstract class Element : IElement
    {
        public int? MinOccurs { get; set; }
        public int? MaxOccurs { get; set; }

        public abstract string Name { get; }

        public static Element Create(XElement element, Dictionary<string, RootElement> elements, Dictionary<string, ElementType> types)
        {

            var minOccurs = ElementType.TryToGetOccursAttribute(element, "minOccurs");
            var maxOccurs = ElementType.TryToGetOccursAttribute(element, "maxOccurs");


            var reference = ElementType.TryToGetAttribute(element, "ref");
            if (reference != null)
            {
                var referencedElement = elements[reference];
                var refResult = new ReferenceElement(referencedElement);
                refResult.MinOccurs = minOccurs;
                refResult.MaxOccurs = maxOccurs;
                return refResult;
            }

            var name = ElementType.GetExpectedAttribute(element, "name");
            var type = ElementType.GetExpectedAttribute(element, "type");

            if (types.ContainsKey(type))
            {
                if (types[type] is ComplexType complexRestriction)
                {
                    return new NamedElement(name, ElementXMLType.ComplexType, complexRestriction);
                }
                else
                {
                    throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (element ELEMENT contain reference to simple type {type}).");
                }
            }
            var valueType = type.ToLower() switch
            {
                "xsd:qname" => ElementXMLType.QName,
                "xsd:idref" => ElementXMLType.IDRef,
                _ => throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (element attribute contain unexpected attribute {type}).")
            };

            return new NamedElement(name, valueType, null);
        }
    }

    public class ReferenceElement :Element
    {
        public RootElement ReferencedElement { get; init; }

        public override string Name => ReferencedElement.Name;

        public ReferenceElement(RootElement referencedElement)
        {
            this.ReferencedElement = referencedElement;
        }

        public override string ToString()
        {
            return $"<refToRootElement {ReferencedElement.Name} min={MinOccurs} max={MaxOccurs}>";
        }
    }

    public class NamedElement : Element
    {
        private string name;
        public override string Name => name;

        public ElementXMLType Category { get; init; }
        public ComplexType? InnerComplexType { get; init; }

        public NamedElement(string name, ElementXMLType category, ComplexType? innerComplexType)
        {
            this.name = name;
            Category = category;
            InnerComplexType = innerComplexType;
        }
        public override string ToString()
        {
            return $"<{Name} category={Category}, {(InnerComplexType is null? "":"type="+InnerComplexType.Name)}, min={MinOccurs},  max={MaxOccurs}>";
        }
    }

    public class ContainerElement : IElement
    {
        public enum ContainerType
        {
            Sequence,
            Choice
        }

        public List<IElement> InnerElements { get; } = new();

        public ContainerType Type { get; init; }

        public ContainerElement(ContainerType type)
        {
            Type = type;
        }


    }

    public class RootElement :NamedElement
    {
        public string? Group { get; init; }

        public ComplexType Type
        {
            get
            {
                if (InnerComplexType == null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (root element should have a complex type).");
                return InnerComplexType;
            }
        }

        public RootElement(string name, ComplexType innerType, string? group) :base(name, ElementXMLType.ComplexType, innerType)
        {
            Group = group;
        }
    }

}
