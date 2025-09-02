using BPMNModel.Model;
using System.Windows;

namespace BPMNVisualizer.Visualization.Renderers
{

    public interface IShapeRenderer
    {
        void RenderShape(BaseElement element, Rect bounds);
    }
}