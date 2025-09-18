namespace BPMNModel.Model
{
    public interface ITraversableNode
    {
        List<ITraversableNode> GetChildElements();

        TResult? Accept<TResult>(IBaseVisitor<TResult> visitor) where TResult : class;
    }
}