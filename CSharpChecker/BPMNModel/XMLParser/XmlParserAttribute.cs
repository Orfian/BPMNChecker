using BPMNModel.XMLElements;
using System.Xml.Linq;
using Utility;

namespace BPMNModel.XMLParser
{
    public class XmlParserAttribute
    {
        private object? processedValue = null;

        public XmlParserAttribute(string name, XName xmlName, string value, AttributeXMLType type)
        {
            Value = value;
            Name = name;
            Type = type;
            XmlName = xmlName;
        }

        public string Name { get; init; }

        public object? ProcessedValue
        {
            get
            {
                switch (Type)
                {
                    case AttributeXMLType.ID: return Value;
                    case AttributeXMLType.String: return Value;
                    case AttributeXMLType.Boolean: return processedValue;
                    case AttributeXMLType.Integer: return processedValue;
                    case AttributeXMLType.Double: return processedValue;
                    case AttributeXMLType.URI:
                        if (processedValue is Uri uri) return uri.ToString();
                        else throw new BPMNCheckerExceptions($"Attribute {Name} of URI type contains {processedValue?.GetType().FullName}).");
                    case AttributeXMLType.SimpleType:
                        if (processedValue is Uri uriInSimpleType) return uriInSimpleType.ToString();
                        else if (processedValue is string valueInSimpleType) return valueInSimpleType.ToString();
                        else throw new BPMNCheckerExceptions($"Attribute {Name} contain unexpected type {processedValue?.GetType().FullName}).");
                    case AttributeXMLType.IDRef:
                    //TODO: QNAME as IDREF now - returning XML node.
                    case AttributeXMLType.QName: return processedValue;
                    default:
                        throw new BPMNCheckerExceptions($"Attribute {Name} wrong type {Type}).");
                }
            }
            set
            {
                processedValue = value;
            }
        }

        public AttributeXMLType Type { get; init; }
        public string Value { get; init; }
        public XName XmlName { get; set; }

        public override string ToString()
        {
            if ((Type == AttributeXMLType.IDRef || Type == AttributeXMLType.QName) && ProcessedValue is not null)
            {
                return $"{Name}=\"{((XmlParserComplexNode)ProcessedValue).ID}\"";
            }
            return $"{Name}=\"{(ProcessedValue is null ? Value : ProcessedValue)}\"";
        }
    }
}