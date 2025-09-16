using BPMNModel.Camunda;

namespace BPMNModel.Model
{
    public interface ITraversableNode
    {
        List<ITraversableNode> GetChildElements();
    }

    public interface IElementWithId
    {
        string? Id { get; }
    }

    public abstract class CamundaExtensionBaseElement
    {
        public List<ICamundaBaseElement> CamundaElements { get; } = new();
    }
}