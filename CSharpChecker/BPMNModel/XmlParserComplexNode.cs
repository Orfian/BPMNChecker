namespace BPMNModel
{
    public class XmlParserComplexNode :XmlParserNode
    {
        private static readonly Dictionary<string, XmlParserComplexNode> cache = new();

        public ComplexType? Type { get; private set; }

        public string ID { get; init; }

        public Dictionary<string, List<XmlParserNode>> ChildNodes { get; } = new();

        private XmlParserComplexNode(string id)
        {
            ID = id;
        }
        private XmlParserComplexNode(string id, ComplexType type)
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

        public static XmlParserComplexNode CreateOrGet(string id, ComplexType complexType)
        {
            if (cache.ContainsKey(id))
            {
                var cached = cache[id];
                cached.Type = complexType;
                return cached;
            }
            else
            {
                var cached = new XmlParserComplexNode(id, complexType);
                cache.Add(id, cached);
                return cached;
            }
        }
        public static XmlParserComplexNode GetReferecne(string id)
        {
            if (cache.ContainsKey(id))
            {
                var cached = cache[id];
                return cached;
            }
            else
            {
                var cached = new XmlParserComplexNode(id);
                cache.Add(id, cached);
                return cached;
            }
        }

        public static string? CheckReferencesWithoutDefinitions()
        {
            var justReference = cache.Where(x => x.Value.Type is null).Select(x => x.Key);

            if (justReference.Any())
            {
                return $"IDs: {string.Join(",", justReference)} were referenced but not defined.";
            }
            return null;
        }
    }
}
