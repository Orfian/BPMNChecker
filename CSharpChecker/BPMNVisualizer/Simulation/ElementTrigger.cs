using BPMNModel.Model;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public class ElementTrigger
{
    public BPMNToken Token { get; set; }
    public BaseElement Element { get; set; }
    
    public Indicator Indicator { get; set; }
    
    public ElementTrigger DeepClone()
    {
        return new ElementTrigger
        {
            Token = this.Token.DeepClone(),
            Element = this.Element, // Note: This is a reference copy. You may want to clone the event if needed.
            Indicator = this.Indicator.DeepClone() // Note: This creates a new indicator and clones the properties.
        };
    }
}