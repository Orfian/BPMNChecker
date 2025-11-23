using System.Windows.Shapes;
using BPMNModel.Model;

namespace BPMNVisualizer.Simulation;

public class BPMNToken
{
    public BaseElement CurrentElement { get; set; }
    public SequenceFlow CurrentSequenceFlow { get; set; }
    public Ellipse Visual { get; set; }
    public bool IsWaiting { get; set; } = false;
    
    public BPMNToken? Parent { get; set; }
}