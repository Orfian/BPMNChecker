namespace BPMNModel
{
    public class XmlParserComplexNode :XmlParserNode
    {
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
        public static XmlParserComplexNode CreatePlaceholderForAnyNodes()
        {
            return new XmlParserComplexNode("any");
        }

        public override string ToString()
        {
            return $"<id={ID} {(Type is null ? "Stub" : Type.Name)} >";
        }
    }
}
