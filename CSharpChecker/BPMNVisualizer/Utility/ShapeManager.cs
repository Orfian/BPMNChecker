using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BPMNVisualizer.Utility
{
    public class ShapeManager
    {
        public TextBlock GetLabel(string text, Rect bounds)
        {
            var label = new TextBlock
            {
                Text = text,
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = bounds.Width,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            return label;
        }
        
        public Rectangle GetRectangle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var rectangle = new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush
            };

            if (dashed)
            {
                rectangle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            return rectangle;
        }
        
        public Rectangle GetRoundedRectangle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var rectangle = new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush,
                RadiusX = 5,
                RadiusY = 5
            };
            
            if (dashed)
            {
                rectangle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            return rectangle;
        }
        
        public Rectangle GetDoubleRoundedRectangle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var outerRectangle = new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush,
                RadiusX = 5,
                RadiusY = 5
            };

            var innerRectangle = new Rectangle
            {
                Width = bounds.Width * 0.8,
                Height = bounds.Height * 0.8,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush,
                RadiusX = 5,
                RadiusY = 5
            };
            
            if (dashed)
            {
                outerRectangle.StrokeDashArray = new DoubleCollection { 4, 2 };
                innerRectangle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            var container = new Grid();
            container.Children.Add(outerRectangle);
            container.Children.Add(innerRectangle);

            innerRectangle.HorizontalAlignment = HorizontalAlignment.Center;
            innerRectangle.VerticalAlignment = VerticalAlignment.Center;

            return outerRectangle;
        }
        
        public Rectangle GetThickRoundedRectangle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var rectangle = new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 6,
                Fill = brush,
                RadiusX = 5,
                RadiusY = 5
            };
            
            if (dashed)
            {
                rectangle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            return rectangle;
        }

        public Ellipse GetCircle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var circle = new Ellipse
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush
            };
            
            if (dashed)
            {
                circle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            return circle;
        }

        public Grid GetDoubleCircle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var outerCircle = new Ellipse
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush
            };

            var innerCircle = new Ellipse
            {
                Width = bounds.Width * 0.8,
                Height = bounds.Height * 0.8,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush
            };
            
            if (dashed)
            {
                outerCircle.StrokeDashArray = new DoubleCollection { 4, 2 };
                innerCircle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            var container = new Grid();
            container.Children.Add(outerCircle);
            container.Children.Add(innerCircle);

            innerCircle.HorizontalAlignment = HorizontalAlignment.Center;
            innerCircle.VerticalAlignment = VerticalAlignment.Center;

            return container;
        }
        
        public Ellipse GetThickCircle(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var circle = new Ellipse
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 4,
                Fill = brush
            };
            
            if (dashed)
            {
                circle.StrokeDashArray = new DoubleCollection { 4, 2 };
            }

            return circle;
        }
        
        public Polygon GetDiamond(Rect bounds, Brush? brush = null, bool dashed = false)
        {
            var diamond = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(bounds.Width/2, 0),
                    new Point(bounds.Width, bounds.Height/2),
                    new Point(bounds.Width/2, bounds.Height),
                    new Point(0, bounds.Height/2)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = brush
            };
            
            if (dashed)
            {
                diamond.StrokeDashArray = new DoubleCollection { 4, 2 };
            }
            
            return diamond;
        }

        public Border WrapInContainer(UIElement element, Rect bounds, double paddingPercentage = 0.0)
        {
            double paddingX = Math.Max(0, bounds.Width * paddingPercentage);
            double paddingY = Math.Max(0, bounds.Height * paddingPercentage);
    
            double containerWidth = Math.Max(0, bounds.Width - (2 * paddingX));
            double containerHeight = Math.Max(0, bounds.Height - (2 * paddingY));

            var container = new Border
            {
                Width = containerWidth,
                Height = containerHeight,
                Child = element,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            return container;
        }
    }
}