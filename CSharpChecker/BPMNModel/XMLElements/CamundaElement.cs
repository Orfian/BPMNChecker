namespace BPMNModel.XMLElements
{
    public class CamundaElement : Element
    {
        private string elementName;

        public CamundaElementType Type { get; init; }

        public CamundaElement(string elementName, CamundaElementType type, bool isMany)
        {
            this.elementName = elementName;
            Type = type;

            MinOccurs = 0;
            MaxOccurs = isMany ? int.MaxValue : 1;
        }

        public override string Name => elementName;

        public override string ToString()
        {
            return $"<{Name} type={Type.CamundaJSonType.Name}, min={MinOccurs},  max={MaxOccurs}>";
        }
    }
}