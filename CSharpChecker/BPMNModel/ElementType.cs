using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public abstract class ElementType
    {
        public static string GetExpectedAttribute(XElement element, string name)
        {
            var nameAttribute = element.Attribute(name);
            if (nameAttribute is null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({element.Name.LocalName} should have attribute {name}).");
            return nameAttribute.Value;
        }
        public static XElement GetExpectedSingleElement(XElement element, string name)
        {
            if (element.Elements().Count() != 1) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({element.Name.LocalName} - should have just one element named {name}).");
            var inner = element.Elements().First();
            if (inner.Name.LocalName != name) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({element.Name.LocalName} - should have just one element named {name} not  {inner.Name.LocalName}).");

            return inner;
        }

        public static void CheckExpectedAttributr(XElement element, string name, string value)
        {
            var nameAttribute = element.Attribute(name);
            if (nameAttribute is null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({element.Name.LocalName} should have attribute {name}).");
            if (nameAttribute.Value != value) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({element.Name.LocalName} should have attribute {name} with value {value} not {nameAttribute.Value}).");
        }
        public static void CheckExpectedElement(XElement element, string name)
        {
            if (element.Name.LocalName != name) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (We are expecting element {element.Name.LocalName} not {name}).");
        }
        public static bool TryToGetBoolAttribute(XElement element, string name)
        {
            var isAttribute = element.Attribute(name);

            if (isAttribute is not null)
            {
                return isAttribute.Value == "true";
            }
            return false;
        }

        public static int? TryToGetOccursAttribute(XElement element, string name)
        {
            var isAttribute = element.Attribute(name);

            if (isAttribute is null) return null;

            if (isAttribute.Value == "unbounded") return int.MaxValue;           
            return int.Parse(isAttribute.Value);
            
        }

        public static string? TryToGetAttribute(XElement element, string name)
        {
            var attribute = element.Attribute(name);
            return attribute?.Value;
        }

        public static void DumpAttributesExcept(XElement element, string[] knownAttributes)
        {

            var att = element.Attributes().Where(x => !knownAttributes.Contains(x.Name.LocalName));
            if (att.Any())
            {
                Console.WriteLine($"{element.Name.LocalName}[Attributes] = {string.Join(" ", att)}");
            }
        }

        public static void DumpElementsExcept(XElement element, string[] knownElements)
        {
            var att = element.Elements().Where(x => !knownElements.Contains(x.Name.LocalName));
            if (att.Any())
            {
                Console.WriteLine($"{element.Name.LocalName}[Elements] = {string.Join(" ", att)}");
            }
        }
    }
}
