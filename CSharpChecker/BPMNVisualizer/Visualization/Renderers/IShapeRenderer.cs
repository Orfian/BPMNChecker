using BPMNModel.Model;
using System.Windows;

namespace BPMNVisualizer.Visualization.Renderers
{

    public interface IShapeRenderer
    {
        void RenderShape(BPMNShape shape);
    }
}