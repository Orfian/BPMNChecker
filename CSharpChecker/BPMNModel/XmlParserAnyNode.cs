using System.Xml.Linq;

namespace BPMNModel
{
    public class XmlParserAnyNode : XmlParserNode
    {
        public XElement AnyNode { get; init; }
        public XmlParserAnyNode(XElement anyNode)
        {
            AnyNode = anyNode;
        }
        public override string ToString()
        {
            return AnyNode.ToString();
        }
    }
}
