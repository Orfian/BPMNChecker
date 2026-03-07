using System.Windows.Shapes;
using BPMNModel.Model;

namespace BPMNVisualizer.Utility;

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