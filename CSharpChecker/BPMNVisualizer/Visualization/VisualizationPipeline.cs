using BPMNModel.Model;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using BPMNModel;

namespace BPMNVisualizer.Visualization
{
    public class VisualizationPipeline
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly RendererFactory _rendererFactory;
        private const double ScaleFactor = 1.0;

        public VisualizationPipeline(ILogger logger, Canvas canvas, RendererFactory rendererFactory)
        {
            _logger = logger;
            _canvas = canvas;
            _rendererFactory = rendererFactory;
        }

        public void Execute(ModelRoot model)
        {
            if (model?.Definition?.Diagrams.FirstOrDefault()?.Plane == null)
            {
                _logger.Warning("No valid diagram found in model");
                return;
            }

            var diagramElements = model.Definition.Diagrams[0].Plane;
            _logger.Information("Starting visualization pipeline");
            
            if (diagramElements == null) return;
            
            CalculateViewport(diagramElements);
            
            // Phase 1: Render pool and lane structures first
            RenderStructuralElements(diagramElements);
            
            // Phase 2: Render main process elements
            RenderProcessElements(diagramElements);
            
            // Phase 3: Render connections last (on top of other elements)
            RenderConnections(diagramElements, model);
        }
        
        private void CalculateViewport(BPMNPlane plane)
        {
            var allBounds = plane.PlaneElement
                .OfType<BPMNShape>()
                .Select(s => s.Bounds.ToRect(ScaleFactor));

            if (!allBounds.Any()) return;

            _canvas.Width = allBounds.Max(b => b.Right) + 50;
            _canvas.Height = allBounds.Max(b => b.Bottom) + 50;
        }

        private void RenderStructuralElements(BPMNPlane plane)
        {
            _logger.Debug("Rendering structural elements");
            foreach (var shape in plane.PlaneElement.OfType<BPMNShape>())
            {
                if (!IsStructuralElement(shape.BpmnElement) || shape.Bounds == null) continue;
                
                var renderer = _rendererFactory.GetStructureRenderer(shape.BpmnElement);
                renderer?.RenderShape(shape.BpmnElement, shape.Bounds.ToRect(ScaleFactor));
            }
        }

        private void RenderProcessElements(BPMNPlane plane)
        {
            _logger.Debug("Rendering process elements");
            foreach (var shape in plane.PlaneElement.OfType<BPMNShape>())
            {
                if (IsStructuralElement(shape.BpmnElement) || shape.Bounds == null) continue;

                var renderer = _rendererFactory.GetShapeRenderer(shape.BpmnElement);
                renderer?.RenderShape(shape.BpmnElement, shape.Bounds.ToRect(ScaleFactor));
            }
        }

        private void RenderConnections(BPMNPlane plane, ModelRoot model)
        {
            _logger.Debug("Rendering connections");
            foreach (var edge in plane.PlaneElement.OfType<BPMNEdge>())
            {
                if (edge.Waypoint.Count < 2) continue;

                var renderer = _rendererFactory.GetConnectionRenderer(edge.BpmnElement);
                renderer?.RenderConnection(edge.BpmnElement, edge.Waypoint.ToPoints(ScaleFactor));
            }
        }

        private bool IsStructuralElement(BaseElement element)
        {
            return element is Participant || element is Lane;
        }
    }

    // Extension methods for coordinate conversion
    internal static class VisualizationExtensions
    {
        public static Rect ToRect(this Bounds bounds, double scale)
        {
            return new Rect(
                (bounds.X ?? 0) * scale,
                (bounds.Y ?? 0) * scale,
                (bounds.Width ?? 100) * scale,
                (bounds.Height ?? 60) * scale);
        }

        public static IEnumerable<System.Windows.Point> ToPoints(
            this IEnumerable<BPMNModel.Model.Point> waypoints, 
            double scale)
        {
            return waypoints.Select(wp => new System.Windows.Point(
                (wp.X ?? 0) * scale,
                (wp.Y ?? 0) * scale
            ));
        }
    }
}