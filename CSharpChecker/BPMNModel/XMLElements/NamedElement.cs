namespace BPMNModel.XMLElements
{
    public class NamedElement : Element
    {
        private string name;

        public NamedElement(string name, ElementXMLType category, ComplexType? innerComplexType, int? minOccurs, int? maxOccurs)
        {
            this.name = name;
            Category = category;
            InnerComplexType = innerComplexType;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
        }

        public ElementXMLType Category { get; init; }
        public ComplexType? InnerComplexType { get; init; }
        public override string Name => name;

        public override string ToString()
        {
            return $"<{Name} category={Category}, {(InnerComplexType is null ? "" : "type=" + InnerComplexType.Name)}, min={MinOccurs},  max={MaxOccurs}>";
        }
    }
}