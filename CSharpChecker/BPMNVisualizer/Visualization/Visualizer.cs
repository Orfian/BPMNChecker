using System.Windows.Controls;
using BPMNModel;
using BPMNModel.Model;

namespace BPMNVisualizer.Visualization
{
    public class Visualizer
    {
        private readonly Canvas _canvas;
        private readonly RendererFactory _rendererFactory;

        public Visualizer(Canvas canvas)
        {
            _canvas = canvas;
            
            _rendererFactory = new RendererFactory(_canvas);
        }
        
        public void Visualize(ModelRoot model)
        {
            _canvas.Children.Clear();
            if (model.Definition?.Diagrams.FirstOrDefault()?.Plane == null) return;

            var visualizationPipeline = new VisualizationPipeline(_canvas, _rendererFactory);
            visualizationPipeline.Execute(model);
        }

        public void VisualizeDiagram(BPMNDiagram diagram)
        {
            _canvas.Children.Clear();
            if (diagram?.Plane == null) return;

            var visualizationPipeline = new VisualizationPipeline(_canvas, _rendererFactory);
            visualizationPipeline.ExecuteDiagram(diagram);
        }
    }
}