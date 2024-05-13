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

    public abstract class Element : IElement
    {
        public int? MinOccurs { get; set; }
        public int? MaxOccurs { get; set; }


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
                    return new NamedElement(name, RestrictedXMLType.ComplexType, complexRestriction, null);
                }
                else
                {
                    throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (element ELEMENT contain reference to simple type {type}).");
                }
            }
            var valueType = type.ToLower() switch
            {
                "xsd:qname" => RestrictedXMLType.QName,
                "xsd:idref" => RestrictedXMLType.IDRef,
                _ => throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (element attribute contain unexpected attribute {type}).")
            };

            return new NamedElement(name, valueType, null, type);
        }
    }

    public class ReferenceElement :Element
    {
        public RootElement ReferencedElement { get; init; }

        public ReferenceElement(RootElement referencedElement)
        {
            this.ReferencedElement = referencedElement;
        }

        public override string ToString()
        {
            return $"<ref={ReferencedElement.Name} min={MinOccurs} max={MaxOccurs}>";
        }
    }

    public class NamedElement : Element
    {
        public string Name {  get; init; }

        public RestrictedXMLType Category { get; init; }
        public ComplexType? InnerComplexType { get; init; }

        public string? InnerStringType { get; init; }

        public NamedElement(string name, RestrictedXMLType category, ComplexType? innerComplexType, string? innerStringType)
        {
            Name = name;
            Category = category;
            InnerComplexType = innerComplexType;
            InnerStringType = innerStringType;
        }
        public override string ToString()
        {
            return $"<{Name} type={Category} type={(InnerComplexType is null? InnerStringType : InnerComplexType.Name)} min={MinOccurs} max={MaxOccurs}>";
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

        public RootElement(string name, ComplexType innerType, string? group) :base(name, RestrictedXMLType.ComplexType, innerType, null)
        {
            Group = group;
        }
    }

}
