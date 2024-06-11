
using System;
using System.Xml;

namespace BPMNModel
{
    public class XmlParserComplexNode :XmlParserNode
    {
        public string? MixedContent { get; set; }

        public XmlParserComplexNode(string id)
        {
            ID = id;
        }

        public XmlParserComplexNode(string id, ComplexType type)
        {
            ID = id;
            Type = type;
        }

        public Dictionary<string, List<XmlParserNode>> ChildNodes { get; } = new();
        public string ID { get; init; }
        public ComplexType? Type { get; set; }
        public static XmlParserComplexNode CreatePlaceholderForAnyNodes(ComplexType type)
        {
            return new XmlParserComplexNode("any", type);
        }

        public override string ToString()
        {
            return $"<id={ID} {(Type is null ? "Stub" : Type.Name)} >";
        }

        public override void DumpNode(StreamWriter writer, string indent)
        {
            if (Type is null)
            {
                writer.Write("<ERROR: Node without type />");
            }
            else
            {
                var newIndent = indent + "  ";
                writer.Write($"{indent}<{Type.Name}");
                writer.Write($" ID=\"{ID}\"");
                if (MixedContent is not null) writer.Write($" MixedContent=\"{MixedContent}\"");
                foreach (var (_,attribute) in Attributes)
                {
                    if (!attribute.Name.Equals("id")) writer.Write($" {attribute.ToString()}");
                }
                writer.WriteLine($"{newIndent}/>");


                foreach (var (name, items) in ChildNodes)
                {
                    writer.WriteLine($"{newIndent}<!--{name}-->");
                    foreach (var item in items)
                    {
                        if (item is XmlParserComplexNode complexNode && complexNode.ID is not null && !complexNode.ID.Equals("any"))
                        {
                            writer.WriteLine($"{newIndent}<{complexNode.Type?.Name} ref=\"{complexNode.ID}\"/>");
                        }
                        else
                        {
                            item.DumpNode(writer, newIndent);
                        }
                    }
                }
                writer.WriteLine($"{indent}<{Type.Name}/>\n");
            }
        }
    }
}
