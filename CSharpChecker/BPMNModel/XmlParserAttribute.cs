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

        public AttributeXMLType Type { get; init; }

        private object? processedValue = null;

        public object? ProcessedValue { get
            {
                if (processedValue == null) return Value;
                return processedValue;
            }
            set
            {
                this.processedValue = value;
            }
        }
        public XmlParserAttribute(string name, string value, AttributeXMLType type)
        {
            Value = value;
            Name = name;
            Type = type;    
        }

        public override string ToString()
        {
            return $"{Name} = {(ProcessedValue is null? Value : ProcessedValue)}";
        }
    }
}
