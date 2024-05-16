using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public abstract class XmlParserNode
    {
        private static readonly Dictionary<string, XmlParserComplexNode> cache = new();

        public List<XmlParserAttribute> Attributes { get; } = new();
    }
}
