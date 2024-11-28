using Utility;

namespace BPMNModel
{
    public class RootElement : NamedElement
    {
        public RootElement(string name, ComplexType innerType, string? group) : base(name, ElementXMLType.ComplexType, innerType, null, null)
        {
            Group = group;
        }

        public string? Group { get; init; }

        public ComplexType Type
        {
            get
            {
                if (InnerComplexType == null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken (root element should have a complex type).");
                return InnerComplexType;
            }
        }
    }
}