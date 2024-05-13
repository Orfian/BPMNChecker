using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public class XmlParserNode
    {
        public List<XmlParserAttribute> Attributes {get;} = new List<XmlParserAttribute>();

        public List<XmlParserNode> ChildNodes { get;} = new List<XmlParserNode>();

        public ComplexType Type {  get; }

        public string ID { get; init; }

        public XmlParserNode(string id, ComplexType type) {
            Type = type;
            ID = id;
        }

    }
}
