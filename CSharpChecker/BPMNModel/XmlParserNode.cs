using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public abstract class XmlParserNode
    {
        public Dictionary<string, XmlParserAttribute> Attributes { get; } = new();


    }
}
