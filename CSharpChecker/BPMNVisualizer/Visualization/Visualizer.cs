using System.Windows;
using System.Windows.Controls;
using BPMNModel;
using BPMNModel.Model;
using Serilog;
using Point = System.Windows.Point;


namespace BPMNVisualizer.Visualization
{
    public class Visualizer
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly RendererFactory _rendererFactory;
        private readonly Dictionary<string, Rect> _objectBounds;
        private readonly Dictionary<string, IEnumerable<Point>> _paths;
        private readonly ModelRoot _model;

        public Visualizer(ILogger logger, Canvas canvas, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths, ModelRoot model)
        {
            _logger = logger;
            _canvas = canvas;
            _objectBounds = objectBounds;
            _paths = paths;
            _model = model;
            
            var shapes = (model?.AllObjectsWithIds?.Values ?? Enumerable.Empty<object>())
                .OfType<BPMNShape>()
                .Where(s => s.BpmnElement != null)
                .ToDictionary(s => s.BpmnElement!.Id, s => s);            
            _rendererFactory = new RendererFactory(_logger, _canvas, _objectBounds, _paths, shapes);
        }

        public void Visualize(ModelRoot model)
        {
            _canvas.Children.Clear();
            if (model.Definition?.Diagrams.FirstOrDefault()?.Plane == null) return;

            var visualizationPipeline = new VisualizationPipeline(_logger, _canvas, _rendererFactory);
            visualizationPipeline.Execute(model);
        }
    }
}