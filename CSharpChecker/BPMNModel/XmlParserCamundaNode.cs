
using BPMNModel.Camunda;

namespace BPMNModel
{
    public class XmlParserCamundaNode : XmlParserNode
    {
        public XmlParserCamundaNode(CamundaElementType type)
        {
            Type = type;
        }

        public Dictionary<string, List<XmlParserNode>> ChildNodes { get; } = new();
        public CamundaElementType Type { get; set; }

        public override string ToString()
        {
            return $"<{Type.CamundaJSonType.Name} >";
        }

        public override void DumpNode(StreamWriter writer, string indent)
        {
            var newIndent = indent + "  ";
            writer.Write($"{indent}<{Type.CamundaJSonType.Name}");
            foreach (var (_, attribute) in Attributes)
            {
                writer.Write($" {attribute.ToString()}");
            }
            writer.WriteLine($"{newIndent}/>");

            foreach (var (name, items) in ChildNodes)
            {
                writer.WriteLine($"{newIndent}<!--{name}-->");
                foreach (var item in items)
                {
                    item.DumpNode(writer, newIndent);
                }
            }
            writer.WriteLine($"{indent}<{Type.CamundaJSonType.Name}/>\n");
        }
    }
}
