namespace BPMNModel.XMLElements
{
    public class ReferenceElement : Element
    {
        public ReferenceElement(RootElement referencedElement, int? minOccurs, int? maxOccurs)
        {
            ReferencedElement = referencedElement;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
        }

        public override string Name => ReferencedElement.Name;
        public RootElement ReferencedElement { get; init; }
        public override string ToString()
        {
            return $"<refToRootElement {ReferencedElement.Name} min={MinOccurs} max={MaxOccurs}>";
        }
    }
}