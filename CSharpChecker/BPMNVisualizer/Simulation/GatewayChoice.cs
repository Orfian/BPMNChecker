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

    public GatewayChoice DeepClone()
    {
        return new GatewayChoice
        {
            Token = this.Token.DeepClone(),
            Gateway = this.Gateway, // Note: This is a reference copy. You may want to clone the gateway if needed.
            OutgoingFlows = this.OutgoingFlows, // Note: This is a reference copy. You may want to create a new list if needed.
            MultiSelect = this.MultiSelect,
            DefaultFlow = this.DefaultFlow, // Note: This is a reference copy. You may want to clone the default flow if needed.
            Indicators = this.Indicators.Select(i => i.DeepClone()).ToList() // Note: This creates a new list and clones the indicators.
        };
    }
}

public class Indicator
{
    public Polygon? Visual { get; set; }
    public SequenceFlow Flow { get; set; }
    public bool Selected { get; set; }

    public Indicator DeepClone()
    {
        return new Indicator
        {
            Visual = null, 
            Flow = this.Flow,
            Selected = this.Selected
        };
    }
}