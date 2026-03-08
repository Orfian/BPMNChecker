using BPMNModel.Model;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using BPMNModel;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Visualization
{
    public class VisualizationPipeline
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly RendererFactory _rendererFactory;

        public VisualizationPipeline(Canvas canvas, RendererFactory rendererFactory)
        {
            _logger = SharedVariables.Instance.Logger;
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
            
            RenderPlane(diagramElements, model);
        }

        public void ExecuteDiagram(BPMNDiagram diagram)
        {
            if (diagram?.Plane == null)
            {
                _logger.Warning("No valid plane found in diagram");
                return;
            }

            _logger.Information("Starting visualization pipeline for sub-process diagram");
            RenderPlane(diagram.Plane, null);
        }

        private void RenderPlane(BPMNPlane diagramElements, ModelRoot? model)
        {
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
                .Select(x=>x.Bounds)
                .Where(bounds => bounds != null)
                .Select(b => b!.ToRect());

            if (!allBounds.Any()) return;

            _canvas.Width = allBounds.Max(b => b.Right) + 50;
            _canvas.Height = allBounds.Max(b => b.Bottom) + 50;
        }

        private void RenderStructuralElements(BPMNPlane plane)
        {
            _logger.Debug("Rendering structural elements");
            foreach (var shape in plane.PlaneElement.OfType<BPMNShape>())
            {
                if (shape.BpmnElement == null || !IsStructuralElement(shape.BpmnElement) || shape.Bounds == null) continue;
                
                var renderer = _rendererFactory.GetStructureRenderer(shape.BpmnElement);
                renderer?.RenderShape(shape);
            }
        }

        private void RenderProcessElements(BPMNPlane plane)
        {
            _logger.Debug("Rendering process elements");
            foreach (var shape in plane.PlaneElement.OfType<BPMNShape>())
            {
                if (shape.BpmnElement == null || IsStructuralElement(shape.BpmnElement) || shape.Bounds == null) continue;

                var renderer = _rendererFactory.GetShapeRenderer(shape.BpmnElement);
                renderer?.RenderShape(shape);
            }
        }

        private void RenderConnections(BPMNPlane plane, ModelRoot? model)
        {
            _logger.Debug("Rendering connections");
            foreach (var edge in plane.PlaneElement.OfType<BPMNEdge>())
            {
                if (edge.Waypoint.Count < 2) continue;

                if (edge.BpmnElement != null)
                {
                    var renderer = _rendererFactory.GetConnectionRenderer(edge.BpmnElement);
                    renderer?.RenderConnection(edge);
                }
            }
        }

        private bool IsStructuralElement(BaseElement element)
        {
            return element is Participant || element is Lane || element is Group;
        }
    }

    // Extension methods for coordinate conversion
    internal static class VisualizationExtensions
    {
        public static Rect ToRect(this Bounds bounds, double scale = 1.0)
        {
            return new Rect(
                (bounds.X ?? 0) * scale,
                (bounds.Y ?? 0) * scale,
                (bounds.Width ?? 100) * scale,
                (bounds.Height ?? 60) * scale);
        }

        public static IEnumerable<System.Windows.Point> ToPoints(this IEnumerable<BPMNModel.Model.Point> waypoints, double scale = 1.0)
        {
            return waypoints.Select(wp => new System.Windows.Point(
                (wp.X ?? 0) * scale,
                (wp.Y ?? 0) * scale
            ));
        }
    }
}