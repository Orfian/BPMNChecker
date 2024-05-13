using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BPMNModel
{
    public class XmlParserAttribute
    {
        public string Value { get; init; }
        public string Name { get; init; }

        public RestrictedXMLType Type { get; init; }

        public object? ProcessedValue { get; set; }
        public XmlParserAttribute(string name, string value, RestrictedXMLType type)
        {
            Value = value;
            Name = name;
            Type = type;    
        }
    }
}
