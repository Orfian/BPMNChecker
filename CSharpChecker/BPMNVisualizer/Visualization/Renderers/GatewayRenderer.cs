using System.Windows;
using System.Windows.Controls;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;

namespace BPMNVisualizer.Visualization.Renderers
{
    public class GatewayRenderer : IShapeRenderer
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly BrushManager _brushManager;
        private readonly ShapeManager _shapeManager;
        private readonly SvgResourceManager _svgResourceManager;

        public GatewayRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager,
            SvgResourceManager svgResourceManager)
        {
            _logger = logger;
            _canvas = canvas;
            _brushManager = brushManager;
            _shapeManager = shapeManager;
            _svgResourceManager = svgResourceManager;
        }

        public void RenderShape(BaseElement element, Rect bounds)
        {
            if (element is not Gateway gateway) return;

            var shape = DrawElement(gateway, bounds);
            Canvas.SetLeft(shape, bounds.Left);
            Canvas.SetTop(shape, bounds.Top);
            _canvas.Children.Add(shape);

            var icon = DrawIcon(gateway, bounds);
            if (icon != null)
            {
                Canvas.SetLeft(icon, bounds.Left + (bounds.Width - icon.Width) / 2);
                Canvas.SetTop(icon, bounds.Top + (bounds.Height - icon.Height) / 2);
                _canvas.Children.Add(icon);
            }

            var label = DrawLabel(gateway, bounds);
            if (label != null)
            {
                Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
                Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
                _canvas.Children.Add(label);
            }
        }

        private UIElement DrawElement(Gateway gateway, Rect bounds)
        {
            var shape = _shapeManager.GetDiamond(bounds, _brushManager.GetGatewayBrush(gateway));
            return shape;
        }

        private Border? DrawIcon(Gateway gateway, Rect bounds)
        {
            var icon = _svgResourceManager.GetGatewayIcon(gateway);
            if (icon == null)
            {
                _logger.Warning("No icon found for gateway type: {GatewayType}", gateway.GetType());
                return null;
            }

            return _shapeManager.WrapInContainer(icon, bounds, 0.25);
        }

        private Border? DrawLabel(Gateway gateway, Rect bounds)
        {
            var text = gateway.Name;
            if (string.IsNullOrEmpty(text))
            {
                _logger.Warning("No label found for gateway type: {GatewayType}", gateway.GetType());
                return null;
            }

            bounds.Width *= 2;
            var label = _shapeManager.GetLabel(text, bounds);

            return _shapeManager.WrapInContainer(label, bounds);
        }
    }
}