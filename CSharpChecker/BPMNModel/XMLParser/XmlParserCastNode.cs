using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BPMNModel.XMLElements;

namespace BPMNModel.XMLParser
{
    public class XmlParserCastNode : XmlParserNode
    {
        public XmlParserCastNode(ComplexType type, string value)
        {
            Type = type;
            Value = value;
        }

        public ComplexType Type { get; init; }
        public string Value { get; init; }

        public override void DumpNode(StreamWriter writer, string indent)
        {
            writer.WriteLine($"{indent}<cast to=\"{Type.Name}\" Value=\"{Value}\" />");
        }

        public override string ToString()
        {
            return $"<cast to={Type.Name} Value=\"{Value}\">";
        }
    }
}
