using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using BPMNModel.Model;
using BPMNVisualizer.Simulation;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using BPMNVisualizer.Visualization.Renderers;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Visualization
{
    public class TokenToVisualConverter : IValueConverter
    {
        private SvgResourceManager _svgResourceManager;
        private BrushManager _brushManager;
        private ShapeManager _shapeManager;

        public TokenToVisualConverter()
        {
            _svgResourceManager = new SvgResourceManager();
            _brushManager = new BrushManager();
            _shapeManager = new ShapeManager();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable<BPMNToken> tokens) return null;

            var stackPanel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            int count = 0;
            const int maxIcons = 6;

            foreach (var token in tokens)
            {
                if (count >= maxIcons)
                {
                    stackPanel.Children.Add(new TextBlock 
                    { 
                        Text = "...", 
                        VerticalAlignment = VerticalAlignment.Center, 
                        Margin = new Thickness(2,0,0,0) 
                    });
                    break;
                }

                if (token.CurrentElement is BaseElement element)
                {
                    var visual = GetVisualForElement(element);
                    if (visual != null)
                    {
                        var flowElem = element as FlowElement;
                        var name = flowElem?.Name ?? element.Id;

                        var grid = new Grid
                        {
                            Width = 24,
                            Height = 24,
                            Margin = new Thickness(2, 0, 2, 0),
                            ToolTip = name,
                            Background = Brushes.Transparent // Ensure hit testing works for tooltip
                        };
                        
                        grid.Children.Add(visual);

                        // Add token indicator
                        var tokenIndicator = new Ellipse
                        {
                            Width = 8,
                            Height = 8,
                            Fill = token.IsWaiting ? Brushes.Orange : Brushes.LimeGreen,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Margin = new Thickness(0,0,-2,-2) 
                        };
                        
                        grid.Children.Add(tokenIndicator);
                        
                        stackPanel.Children.Add(grid);
                        count++;
                    }
                }
            }

            return stackPanel;
        }

        private FrameworkElement? GetVisualForElement(BaseElement element)
        {
            var canvas = new Canvas();
            
            IShapeRenderer? renderer = null;
            Bounds bounds = new Bounds();

            switch (element)
            {
                case Activity:
                    renderer = new ActivityRenderer(canvas, _brushManager, _shapeManager, _svgResourceManager, true);
                    // Standard bounds for tasks/activities
                    bounds.X = 0;
                    bounds.Y = 0;
                    bounds.Width = 100;
                    bounds.Height = 80;
                    break;
                case Event:
                    renderer = new EventRenderer(canvas, _brushManager, _shapeManager, _svgResourceManager, true);
                    // Standard bounds for events
                    bounds.X = 0;
                    bounds.Y = 0;
                    bounds.Width = 36;
                    bounds.Height = 36;
                    break;
                case Gateway:
                    renderer = new GatewayRenderer(canvas, _brushManager, _shapeManager, _svgResourceManager, true);
                    // Standard bounds for gateways
                    bounds.X = 0;
                    bounds.Y = 0;
                    bounds.Width = 50;
                    bounds.Height = 50;
                    break;
                default:
                    return null;
            }

            // Render the shape onto the canvas
            renderer.RenderShape(new BPMNShape
            {
                Id = element.Id,
                BpmnElement = element,
                Bounds = bounds
            });

            // Wrap in Viewbox to scale down uniformly
            var viewbox = new Viewbox
            {
                Child = canvas,
                Stretch = Stretch.Uniform,
                Width = 24, 
                Height = 24
            };
            
            // Set canvas size explicitly so Viewbox knows the aspect ratio
            canvas.Width = bounds.Width ?? 0;
            canvas.Height = bounds.Height ?? 0;

            return viewbox;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
