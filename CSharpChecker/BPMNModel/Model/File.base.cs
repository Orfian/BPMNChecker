using BPMNModel.Camunda;

namespace BPMNModel.Model
{

    public interface IElementWithId
    {
        string? Id { get; }
    }

    public abstract class CamundaExtensionBaseElement
    {
        public List<ICamundaBaseElement> CamundaElements { get; } = new();
    }
}