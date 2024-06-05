namespace BPMNModel
{
    public class XmlParserComplexNode :XmlParserNode
    {
        public ComplexType? Type { get; set; }

        public string ID { get; init; }

        public Dictionary<string, List<XmlParserNode>> ChildNodes { get; } = new();

        public XmlParserComplexNode(string id)
        {
            ID = id;
        }
        public XmlParserComplexNode(string id, ComplexType type)
        {
            ID = id;
            Type = type;
        }

        public override string ToString()
        {
            return $"<id={ID} {(Type is null ? "Stub" : Type.Name)} >";
        }

        public static XmlParserComplexNode CreatePlaceholderForAnyNodes()
        {
            return new XmlParserComplexNode("any");
        }

        
    }
}
