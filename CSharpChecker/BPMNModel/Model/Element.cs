
namespace BPMNModel.Model
{
    public class Element : ITraversableNode
    {
        public Element(string value)
        {
            Value = value;
        }

        public string Value { get; init; }

        public TResult? Accept<TResult>(IBaseVisitor<TResult> visitor) where TResult : class
        {
            throw new NotImplementedException();
        }

        public void Enter([NotNull] IBaseListener listener)
        {
            throw new NotImplementedException();
        }

        public void Exit([NotNull] IBaseListener listener)
        {
            throw new NotImplementedException();
        }

        public List<ITraversableNode> GetChildElements()
        {
            return new List<ITraversableNode>();
        }
    }
}