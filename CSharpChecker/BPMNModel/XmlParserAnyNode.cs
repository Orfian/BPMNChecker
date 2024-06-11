using System.Xml;
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

        public override void DumpNode(StreamWriter writer, string indent)
        {
            writer.WriteLine($"{indent}<any>");
            var xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;
            AnyNode.WriteTo(xmlWriter);
            writer.WriteLine($"{indent}</any>");
        }

        public override string ToString()
        {
            return AnyNode.ToString();
        }
    }
}