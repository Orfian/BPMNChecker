using BPMNModel.Camunda;
using BPMNModel.XMLElements;

namespace BPMNModel.XMLParser
{
    public class XmlParserCamundaNode : XmlParserNode
    {
        public XmlParserCamundaNode(CamundaElementType type)
        {
            Type = type;
        }

        public Dictionary<string, List<XmlParserNode>> ChildNodes { get; } = new();
        public CamundaElementType Type { get; set; }

        public override void DumpNode(StreamWriter writer, string indent)
        {
            var newIndent = indent + "  ";
            writer.Write($"{indent}<{Type.CamundaJSonType.Name}");
            foreach (var (_, attribute) in Attributes)
            {
                writer.Write($" {attribute.ToString()}");
            }
            writer.WriteLine($"/>");

            foreach (var (name, items) in ChildNodes)
            {
                writer.WriteLine($"{newIndent}<!--{name}-->");
                foreach (var item in items)
                {
                    item.DumpNode(writer, newIndent);
                }
            }
            writer.WriteLine($"{indent}<{Type.CamundaJSonType.Name}/>");
        }

        public override string ToString()
        {
            return $"<{Type.CamundaJSonType.Name} >";
        }
    }
}