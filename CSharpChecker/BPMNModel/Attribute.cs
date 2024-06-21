using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public enum AttributeUse
    {
        Required,
        Optional
    }

    public enum AttributeXMLType
    {
        ID,
        String,
        Boolean,
        Integer,
        URI,
        SimpleType,
        QName,
        IDRef,
    }

    public class Attribute
    {
        public Attribute(string name, (AttributeXMLType Category, SimpleType? Restriction) type)
        {
            Name = name;
            Type = type;
            Use = AttributeUse.Optional;
        }

        public string? Default { get; set; }
        public string Name { get; init; }

        public (AttributeXMLType Category, SimpleType? Restriction) Type { get; init; }

        public AttributeUse Use { get; set; }
        public static (AttributeXMLType Category, SimpleType? Restriction) LoadTypeFromString(string type, Dictionary<string, ElementType> types)
        {
            if (types.ContainsKey(type))
            {
                if (types[type] is SimpleType restriction)
                {
                    return (AttributeXMLType.SimpleType, restriction);
                }
            }

            return type.ToLower() switch
            {
                "xsd:qname" => (AttributeXMLType.QName, null),
                "xsd:boolean" => (AttributeXMLType.Boolean, null),
                "xsd:integer" => (AttributeXMLType.Integer, null),
                "xsd:idref" => (AttributeXMLType.IDRef, null),
                "xsd:id" => (AttributeXMLType.ID, null),
                "xsd:string" => (AttributeXMLType.String, null),
                "xsd:anyuri" => (AttributeXMLType.URI, null),
                "xsd:int" => (AttributeXMLType.Integer, null),
                _ => throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (element attribute contain unexpected attribute {type}).")
            };
        }

        public (XmlParserAttribute? Result, string? Error) CreateAndCheck(XElement element, XmlParser parser)
        {
            var convertedName = parser.GetXName(Name);

            var attributeFromXml = element.Attribute(convertedName);

            if (attributeFromXml == null)
            {
                if (Use == AttributeUse.Required)
                {
                    return (null, $"Required attribute {Name} is missing.");
                }
                if (Default is null)
                {
                    return (null, null);
                }
            }

            string realValue = attributeFromXml is null ? Default ?? String.Empty : attributeFromXml.Value;
            var resultingAttribute = new XmlParserAttribute(Name, convertedName ,realValue, Type.Category);
            var realValueinLowerCase = realValue.ToLower();

            switch (this.Type.Category)
            {
                case AttributeXMLType.ID: return (resultingAttribute, null);
                case AttributeXMLType.String: return (resultingAttribute, null);
                case AttributeXMLType.Boolean:
                    if (realValueinLowerCase != "true" && realValueinLowerCase != "false") return (resultingAttribute, $"Wrong boolean value: {realValue}");
                    resultingAttribute.ProcessedValue = realValueinLowerCase == "true" ? true : false;
                    return (resultingAttribute, null);

                case AttributeXMLType.Integer:
                    if (long.TryParse(realValue, out long value))
                    {
                        resultingAttribute.ProcessedValue = value;
                        return (resultingAttribute, null);
                    }
                    else
                    {
                        return (resultingAttribute, $"Wrong integer value: {realValue}");
                    }
                case AttributeXMLType.URI:
                    if (Uri.TryCreate(realValue, UriKind.RelativeOrAbsolute, out Uri? uri))
                    {
                        resultingAttribute.ProcessedValue = uri;
                        return (resultingAttribute, null);
                    }
                    else
                    {
                        return (resultingAttribute, $"Wrong URI value: {realValue}");
                    }

                case AttributeXMLType.SimpleType:
                    if (Type.Restriction is null) return (resultingAttribute, $"Attached simple type not found.");
                    else
                    {
                        resultingAttribute.ProcessedValue = Type.Restriction.Check(realValue, out string? error);
                        return (resultingAttribute, error);
                    }
                case AttributeXMLType.IDRef:
                //TODO: QNAME as IDREF now.
                case AttributeXMLType.QName:
                    var node = parser.GetReferecne(realValue);
                    resultingAttribute.ProcessedValue = node;
                    return (resultingAttribute, null);

                default:
                    return (null, $"Unsolved attribute {Name} with type: " + this.Type.Category);
            }
        }

        public override string ToString()
        {
            return $"Attribute({Name} type={Type.Category}, restriction={Type.Restriction?.Name}, use={Use}, default={Default})";
        }
    }
}