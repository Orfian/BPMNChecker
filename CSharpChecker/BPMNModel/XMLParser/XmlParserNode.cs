using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.XMLParser
{
    public abstract class XmlParserNode
    {
        public SortedDictionary<string, XmlParserAttribute> Attributes { get; } = new();

        public abstract void DumpNode(StreamWriter writer, string indent);
    }
}