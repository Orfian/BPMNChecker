using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel.XMLElements
{
    public class SimpleType : ElementType
    {
        private SimpleType(string name, string[] restrictedValues, bool acceptsURL)
        {
            Name = name;
            RestrictedValues = restrictedValues;
            AcceptsURL = acceptsURL;
        }

        public bool AcceptsURL { get; init; }
        public string Name { get; init; }
        public string[] RestrictedValues { get; init; }
        public static SimpleType Create(XNamespace xs, XElement element)
        {
            string[] GetRestrictions(XElement elem)
            {
                List<string> values = new List<string>();

                foreach (XElement value in elem.Elements())
                {
                    CheckExpectedElement(value, "enumeration");
                    values.Add(GetExpectedAttribute(value, "value"));
                }

                return values.ToArray();
            }

            var name = GetExpectedAttribute(element, "name");

            if (element.Element(xs + "restriction") is not null)
            {
                var restriction = GetExpectedSingleElement(element, "restriction");
                CheckExpectedAttributr(restriction, "base", "xsd:string");
                SimpleType result = new SimpleType(name, GetRestrictions(restriction), false);
                return result;
            }
            else
            {
                var union = GetExpectedSingleElement(element, "union");
                CheckExpectedAttributr(union, "memberTypes", "xsd:anyURI");
                var simpleType = GetExpectedSingleElement(union, "simpleType");
                var restriction = GetExpectedSingleElement(simpleType, "restriction");
                CheckExpectedAttributr(restriction, "base", "xsd:token");
                SimpleType result = new SimpleType(name, GetRestrictions(restriction), true);
                return result;
            }
        }

        public object? Check(string value, out string? error)
        {
            if (RestrictedValues.Contains(value))
            {
                error = null;
                return value;
            }

            if (AcceptsURL && Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri? uri))
            {
                error = null;
                return uri;
            }

            error = $"Unexpected value {value}, need: {string.Join(",", RestrictedValues)}{(AcceptsURL ? " or URL" : "")}.";
            return null;
        }
    }
}