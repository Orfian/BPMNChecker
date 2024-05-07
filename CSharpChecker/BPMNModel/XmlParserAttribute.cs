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
        public XmlParserAttribute(string name, string value)
        {
            Value = value;
            Name = name;
        }
    }
}
