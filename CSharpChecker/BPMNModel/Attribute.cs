using System.Xml.Linq;
using Utility;

namespace BPMNModel
{

    public enum AttributeUse
    {
        Required,
        Optional
    }

    public enum RestrictedXMLType
    {
        QName,
        Boolean,
        Integer,
        IDRef,
        SimpleType,
        ComplexType,
        ID,
        String,
        URI,
        
    }

    public class Attribute
    {

        public string Name { get; init; }

        public (RestrictedXMLType Category, SimpleType? Restriction) Type { get; init; }

        public AttributeUse Use { get; set; }

        public string? Default { get; set; }

        public Attribute(string name, (RestrictedXMLType Category, SimpleType? Restriction) type)
        {
            Name = name;
            Type = type;
            Use = AttributeUse.Optional;
        }

        public static (RestrictedXMLType Category, SimpleType? Restriction) LoadTypeFromString(string type, Dictionary<string, ElementType> types)
        {
            if (types.ContainsKey(type))
            {
                if (types[type] is SimpleType restriction)
                {
                    return (RestrictedXMLType.SimpleType,  restriction);
                }
            }

            return type.ToLower() switch
            {
                "xsd:qname" => (RestrictedXMLType.QName, null),
                "xsd:boolean" => (RestrictedXMLType.Boolean, null),
                "xsd:integer" => (RestrictedXMLType.Integer, null),
                "xsd:idref" => (RestrictedXMLType.IDRef, null),
                "xsd:id" => (RestrictedXMLType.ID, null),
                "xsd:string" => (RestrictedXMLType.String, null),
                "xsd:anyuri" => (RestrictedXMLType.String, null),
                "xsd:int" => (RestrictedXMLType.Integer, null),
                _ => throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (element attribute contain unexpected attribute {type}).")
            };
        }

        public (XmlParserAttribute? Result, string? Error)  CreateAndCheck(XElement element)
        {
            var attributeFromXml = element.Attribute(Name);

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

            string realValue = attributeFromXml is null ? Default??String.Empty : attributeFromXml.Value;
            var resultingAttribute = new XmlParserAttribute(Name, realValue, Type.Category);
            var realValueinLowerCase = realValue.ToLower();

            switch (this.Type.Category)
            {
                case RestrictedXMLType.ID: return (resultingAttribute, null);
                case RestrictedXMLType.String: return (resultingAttribute, null);
                case RestrictedXMLType.Boolean: 
                    if (realValueinLowerCase!="true" && realValueinLowerCase!="false") return (resultingAttribute, $"Wrong boolean value: {realValue}"); 
                    resultingAttribute.ProcessedValue= realValueinLowerCase == "true"? true: false;
                    return (resultingAttribute, null);

                default:
                    return (null, $"Unsolved attribute {Name} with type: " + this.Type.Category);
            }
        }


        public override string ToString()
        {
            return $"<{Name} type={Type.Category} restriction={Type.Restriction?.Name} use={Use} default={Default}>";
        }
    }
}
