using BPMNModel.Model;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public class EventTrigger
{
    public BPMNToken Token { get; set; }
    public Event Event { get; set; }
    
    public Indicator Indicator { get; set; }
    
    public EventTrigger DeepClone()
    {
        return new EventTrigger
        {
            Token = this.Token.DeepClone(),
            Event = this.Event, // Note: This is a reference copy. You may want to clone the event if needed.
            Indicator = this.Indicator.DeepClone() // Note: This creates a new indicator and clones the properties.
        };
    }
}