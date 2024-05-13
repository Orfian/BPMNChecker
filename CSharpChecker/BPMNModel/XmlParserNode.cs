using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public class XmlParserNode
    {
        private static readonly Dictionary<string, XmlParserNode> cache = new();

        public List<XmlParserAttribute> Attributes { get; } = new();

        public Dictionary<string, List<XmlParserNode>> ChildNodes { get; } = new();

        public ComplexType? Type { get; private set; }

        public string ID { get; init; }

        private XmlParserNode(string id) {
            ID = id;
        }
        private XmlParserNode(string id, ComplexType type)
        {
            ID = id;
            Type = type;
        }


        public static XmlParserNode CreateOrGet(string id, ComplexType complexType) { 
            if (cache.ContainsKey(id))
            {
                var cached = cache[id];
                cached.Type = complexType;
                return cached;
            }else
            {
                var cached = new XmlParserNode(id, complexType);
                cache.Add(id, cached);
                return cached;
            }
        }
        public static XmlParserNode GetReferecne(string id)
        {
            if (cache.ContainsKey(id))
            {
                var cached = cache[id];
                return cached;
            }
            else
            {
                var cached = new XmlParserNode(id);
                cache.Add(id, cached);
                return cached;
            }
        }

        public static string? CheckReferencesWithoutDefinitions()
        {
            var justReference = cache.Where(x => x.Value.Type is null).Select(x=>x.Key);

            if (justReference.Any())
            {
                return $"IDs: {string.Join(",", justReference)} were referenced but not defined.";
            }
            return null;
        }
        public override string ToString()
        {
            return $"<id={ID} {(Type is null? "Stub":Type.Name)} >";
        }
    }
}
