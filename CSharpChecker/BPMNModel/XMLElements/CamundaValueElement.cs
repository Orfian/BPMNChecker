namespace BPMNModel.XMLElements
{
    public class CamundaValueElement : Element
    {
        private string name;

        public CamundaValueElement(string name)
        {
            this.name = name;
            MinOccurs = 0;
            MaxOccurs = 1;
        }

        public override string Name => name;

        public override string ToString()
        {
            return $"<{Name} type=String min={MinOccurs} max={MaxOccurs}>";
        }

    }
}