using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using BPMNVisualizer.Utilities;

namespace BPMNVisualizer.Visualization.Renderers
{
    public class EventRenderer : IShapeRenderer
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly BrushManager _brushManager;
        private readonly ShapeManager _shapeManager;
        private readonly SvgResourceManager _svgResourceManager;

        public EventRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager, SvgResourceManager svgResourceManager)
        {
            _logger = logger;
            _canvas = canvas;
            _brushManager = brushManager;
            _shapeManager = shapeManager;
            _svgResourceManager = svgResourceManager;
        }
        
        public void RenderShape(BaseElement element, Rect bounds)
        {
            if (element is not Event evt) return;
            
            var shape = DrawElement(evt, bounds);
            Canvas.SetLeft(shape, bounds.Left);
            Canvas.SetTop(shape, bounds.Top);
            _canvas.Children.Add(shape);
            
            var icon = DrawIcon(evt, bounds);
            if (icon != null)
            {
                Canvas.SetLeft(icon, bounds.Left + (bounds.Width - icon.Width) / 2);
                Canvas.SetTop(icon, bounds.Top + (bounds.Height - icon.Height) / 2);
                _canvas.Children.Add(icon);
            }
            
            var label = DrawLabel(evt, bounds);
            if (label != null)
            {
                Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
                Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
                _canvas.Children.Add(label);
            }
        }

        private UIElement DrawElement(Event evt, Rect bounds)
        {
            return evt switch
            {
                StartEvent start =>
                    _shapeManager.GetCircle(bounds, _brushManager.GetEventBrush(evt), start.IsInterrupting == false),
                IntermediateCatchEvent or IntermediateThrowEvent =>
                    _shapeManager.GetDoubleCircle(bounds, _brushManager.GetEventBrush(evt)),
                BoundaryEvent boundary =>
                    _shapeManager.GetDoubleCircle(bounds, _brushManager.GetEventBrush(evt), boundary.CancelActivity == false),
                EndEvent =>
                    _shapeManager.GetThickCircle(bounds, _brushManager.GetEventBrush(evt)),
                _ =>
                    _shapeManager.GetCircle(bounds, _brushManager.GetEventBrush(evt))
            };
        }
        
        private Border? DrawIcon(Event evt, Rect bounds)
        {
            var icon = _svgResourceManager.GetEventIcon(evt);
            if (icon == null)
            {
                _logger.Warning("No icon found for event type: {EventType}", evt.GetType());
                return null;
            }
            return _shapeManager.WrapInContainer(icon, bounds, 0.25);
        }
        
        private Border? DrawLabel(Event evt, Rect bounds)
        {
            var text = evt.Name;
            if (string.IsNullOrEmpty(text))
            {
                _logger.Warning("No label found for event type: {EventType}", evt.GetType());
                return null;
            }
            
            bounds.Width *= 2;
            var label = _shapeManager.GetLabel(text, bounds);
            
            return _shapeManager.WrapInContainer(label, bounds);
        }
    }
}