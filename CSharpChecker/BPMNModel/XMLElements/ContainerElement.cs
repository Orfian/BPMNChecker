namespace BPMNModel.XMLElements
{
    public class ContainerElement
    {
        public ContainerElement()
        {
        }

        public List<Element> InnerElements { get; } = new();

        public List<(string A, string B)> Restrictions { get; } = new();

        public void AddWithRestriction(Element newElement)
        {
            foreach (var oldElement in InnerElements)
            {
                Restrictions.Add((oldElement.Name, newElement.Name));
            }
            InnerElements.Add(newElement);
        }

        public void AddWithRestriction(ContainerElement sequence)
        {
            foreach (var newElement in sequence.InnerElements)
            {
                foreach (var oldElement in InnerElements)
                {
                    Restrictions.Add((oldElement.Name, newElement.Name));
                }
            }
            foreach (var newElement in sequence.InnerElements)
            {
                InnerElements.Add(newElement);
            }
        }
    }
}