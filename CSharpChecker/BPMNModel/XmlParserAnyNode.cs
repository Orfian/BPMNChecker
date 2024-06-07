using System.Xml.Linq;

namespace BPMNModel
{
    public class XmlParserAnyNode : XmlParserNode
    {
        public XmlParserAnyNode(XElement anyNode)
        {
            AnyNode = anyNode;
        }

        public XElement AnyNode { get; init; }
        public override string ToString()
        {
            return AnyNode.ToString();
        }
    }
}