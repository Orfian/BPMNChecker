namespace BPMNModel
{
    public class AnyElement : Element
    {
        public AnyElement(AnyElementNamespace @namespace, int? minOccurs, int? maxOccurs)
        {
            Namespace = @namespace;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
        }

        public override string Name => "any";
        public AnyElementNamespace Namespace { get; init; }
        public override string ToString()
        {
            return $"<any namespace={Namespace} min={MinOccurs} max={MaxOccurs}>";
        }
    }
}