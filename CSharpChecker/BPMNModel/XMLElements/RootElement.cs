using Utility;

namespace BPMNModel.XMLElements
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
                if (InnerComplexType == null) throw new BPMNCheckerExceptions($"Files with XSD definitions are broken (root element should have a complex type).");
                return InnerComplexType;
            }
        }
    }
}