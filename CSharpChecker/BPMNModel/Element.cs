using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    /*
    public interface IElement
    {
    }
    */

    public enum AnyElementNamespace
    {
        Any,
        Other,
    }

    public enum ElementXMLType
    {
        QName,
        IDRef,
        ComplexType,
    }
    public class AnyElement : Element
    {
        public AnyElement(AnyElementNamespace @namespace, int? minOccurs, int? maxOccurs)
        {
            Namespace = @namespace;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
        }

        public override string Name => "any";
        public AnyElementNamespace Namespace { get; init; }
        public override string ToString()
        {
            return $"<any namespace={Namespace} min={MinOccurs} max={MaxOccurs}>";
        }
    }

    public abstract class Element
    {
        public int? MaxOccurs { get; set; }
        public int? MinOccurs { get; set; }
        public abstract string Name { get; }

        public static Element Create(XElement element, Dictionary<string, RootElement> elements, Dictionary<string, ElementType> types)
        {
            var minOccurs = ElementType.TryToGetOccursAttribute(element, "minOccurs");
            var maxOccurs = ElementType.TryToGetOccursAttribute(element, "maxOccurs");

            if (minOccurs is null && maxOccurs is not null)
            {
                minOccurs = 1;
            }

            if (minOccurs is not null && maxOccurs is null)
            {
                maxOccurs = 1;
            }

            if (minOccurs is null && maxOccurs is null)
            {
                minOccurs = 0;
                maxOccurs = 1;
            }

            if (element.Name.LocalName == "any")
            {
                var nameSpace = ElementType.GetExpectedAttribute(element, "namespace");
                var anyElement = new AnyElement(nameSpace == "##any" ? AnyElementNamespace.Any : AnyElementNamespace.Other, minOccurs, maxOccurs);
                return anyElement;
            }

            var reference = ElementType.TryToGetAttribute(element, "ref");
            if (reference != null)
            {
                var referencedElement = elements[reference];
                var refResult = new ReferenceElement(referencedElement, minOccurs, maxOccurs);
                return refResult;
            }

            var name = ElementType.GetExpectedAttribute(element, "name");
            var type = ElementType.GetExpectedAttribute(element, "type");

            if (types.ContainsKey(type))
            {
                if (types[type] is ComplexType complexRestriction)
                {
                    return new NamedElement(name, ElementXMLType.ComplexType, complexRestriction, minOccurs, maxOccurs);
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

            return new NamedElement(name, valueType, null, minOccurs, maxOccurs);
        }
    }

    public class NamedElement : Element
    {
        private string name;
        public NamedElement(string name, ElementXMLType category, ComplexType? innerComplexType, int? minOccurs, int? maxOccurs)
        {
            this.name = name;
            Category = category;
            InnerComplexType = innerComplexType;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
        }

        public ElementXMLType Category { get; init; }
        public ComplexType? InnerComplexType { get; init; }
        public override string Name => name;
        public override string ToString()
        {
            return $"<{Name} category={Category}, {(InnerComplexType is null ? "" : "type=" + InnerComplexType.Name)}, min={MinOccurs},  max={MaxOccurs}>";
        }
    }

    public class ReferenceElement : Element
    {
        public ReferenceElement(RootElement referencedElement, int? minOccurs, int? maxOccurs)
        {
            this.ReferencedElement = referencedElement;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
        }

        public override string Name => ReferencedElement.Name;
        public RootElement ReferencedElement { get; init; }
        public override string ToString()
        {
            return $"<refToRootElement {ReferencedElement.Name} min={MinOccurs} max={MaxOccurs}>";
        }
    }
    public class RootElement : NamedElement
    {
        public RootElement(string name, ComplexType innerType, string? group) : base(name, ElementXMLType.ComplexType, innerType, null, null)
        {
            Group = group;
        }

        public string? Group { get; init; }

        public ComplexType Type
        {
            get
            {
                if (InnerComplexType == null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (root element should have a complex type).");
                return InnerComplexType;
            }
        }
    }
}