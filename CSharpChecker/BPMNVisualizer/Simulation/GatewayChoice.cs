using System.Windows.Shapes;
using BPMNModel.Model;

namespace BPMNVisualizer.Simulation;

public class GatewayChoice
{
    public BPMNToken Token { get; set; }
    public Gateway Gateway { get; set; }
    public IEnumerable<SequenceFlow> OutgoingFlows { get; set; }
    public bool MultiSelect { get; set; }
    public SequenceFlow? DefaultFlow { get; set; }
    
    public List<Indicator> Indicators { get; set; } = new();
}

public class Indicator
{
    public Polygon Visual { get; set; }
    public SequenceFlow Flow { get; set; }
    public bool Selected { get; set; }
}