
namespace BPMNModel.Model
{
    public class Element : ITraversableNode
    {
        public Element(string value)
        {
            Value = value;
        }

        public string Value { get; init; }

        public List<ITraversableNode> GetChildElements()
        {
            return new List<ITraversableNode>();
        }
    }
}