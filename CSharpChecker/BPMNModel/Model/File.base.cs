using BPMNModel.Camunda;

namespace BPMNModel.Model
{
    public class CamundaExtensionBaseElement
    {
        public List<ICamundaBaseElement> CamundaElements { get; } = new();
    }

}
