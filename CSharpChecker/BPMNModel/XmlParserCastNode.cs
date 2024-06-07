using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public class XmlParserCastNode : XmlParserNode
    {
        public XmlParserCastNode(ComplexType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public ComplexType Type { get; init; }
        public string Value { get; init; }
        public override string ToString()
        {
            return $"<cast to={Type.Name} Value=\"{Value}\">";
        }
    }
}
