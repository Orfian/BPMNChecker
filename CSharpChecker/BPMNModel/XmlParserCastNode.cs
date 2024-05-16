using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public class XmlParserCastNode : XmlParserNode
    {
        public string Value { get; init; }

        public ComplexType Type { get; init; }

        public XmlParserCastNode(ComplexType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
        public override string ToString()
        {
            return $"<cast to={Type.Name} Value=\"{Value}\">";
        }
    }
}
