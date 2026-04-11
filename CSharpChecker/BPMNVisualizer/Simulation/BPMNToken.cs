using System.Windows.Shapes;
using BPMNModel.Model;

namespace BPMNVisualizer.Simulation;

public class BPMNToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public FlowNode CurrentElement { get; set; }
    public SequenceFlow CurrentSequenceFlow { get; set; }
    public Ellipse? Visual { get; set; }
    public bool IsWaiting { get; set; } = false;
    public bool IsEvaluated { get; set; } = false;
    
    public BPMNToken? Parent { get; set; }
    
    public TokenManager? Owner { get; set; }
    
    public BPMNToken DeepClone()
    {
        return new BPMNToken
        {
            Id = this.Id, // Keep ID to preserve identity across snapshots
            CurrentElement = this.CurrentElement,
            CurrentSequenceFlow = this.CurrentSequenceFlow,
            Visual = null, 
            IsWaiting = this.IsWaiting,
            IsEvaluated = this.IsEvaluated,
            Parent = this.Parent, // Note: This is a reference copy. You may want to clone the parent token if needed.
            Owner = null // Visual owner is not cloned; will be reassigned when token is recreated
        };
    }
}