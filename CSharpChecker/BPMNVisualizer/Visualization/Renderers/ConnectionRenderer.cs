using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Visualization.Renderers
{
    public class ConnectionRenderer
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly Dictionary<string, IEnumerable<Point>> _paths;

        public ConnectionRenderer(ILogger logger, Canvas canvas, Dictionary<string, IEnumerable<Point>> paths)
        {
            _logger = logger;
            _canvas = canvas;
            _paths = paths;
        }

        public void RenderConnection(BaseElement element, IEnumerable<Point> points)
        {
            switch (element)
            {
                case SequenceFlow flow:
                    RenderSequenceFlow(flow, points);
                    break;
                case MessageFlow messageFlow:
                    RenderMessageFlow(messageFlow, points);
                    break;
                case Association association:
                    RenderAssociation(association, points);
                    break;
            }
            
            _paths[element.Id] = points;
        }
        
        private void RenderSequenceFlow(SequenceFlow flow, IEnumerable<Point> points)
        {
            var visual = CreateBaseConnection(points, []);
        
            if (Helpers.IsDefaultFlow(flow))
                AddDefaultFlowMarker(points.First(), points.ElementAt(1));
        
            if (Helpers.IsConditionalFlow(flow))
                AddConditionalFlowMarker(points.First(), points.ElementAt(1));

            AddArrowhead(points.Last(), Helpers.GetDirection(points.ElementAt(points.Count() - 2), points.Last()));
            AddFlowLabel(flow, points);
        }

        private void RenderMessageFlow(MessageFlow messageFlow, IEnumerable<Point> points)
        {
            var visual = CreateBaseConnection(points, [4.0, 2.0]);
            
            AddOpenArrowhead(points.Last(), Helpers.GetDirection(points.ElementAt(points.Count() - 2), points.Last()));
            AddMessageLabel(messageFlow, points);
        }

        private void RenderAssociation(Association association, IEnumerable<Point> points)
        {
            var visual = CreateBaseConnection(points, [4.0, 2.0]);
            
            AddDottedCircleMarker(points.First());
        
            if (association.AssociationDirection == AssociationDirection.One ||
                association.AssociationDirection == AssociationDirection.Both)
            {
                AddArrowhead(points.Last(), Helpers.GetDirection(points.ElementAt(points.Count() - 2), points.Last()));
            }
        }
        
        private Polyline CreateBaseConnection(IEnumerable<Point> points, DoubleCollection dashArray)
        {
            var polyline = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.5,
                StrokeDashArray = dashArray,
                Points = new PointCollection(points)
            };

            _canvas.Children.Add(polyline);
            
            return polyline;
        }
        
        private void AddDefaultFlowMarker(Point start, Point end)
        {
            var direction = end - start;
            direction.Normalize();

            var markerPos = start + direction * 10;

            var crossLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.5,
                X1 = markerPos.X - 3,
                Y1 = markerPos.Y - 7,
                X2 = markerPos.X + 3,
                Y2 = markerPos.Y + 7
            };

            var angle = Math.Atan2(direction.Y, direction.X) * 180 / Math.PI;
            var rotateTransform = new RotateTransform(angle, markerPos.X, markerPos.Y);
            crossLine.RenderTransform = rotateTransform;

            _canvas.Children.Add(crossLine);
        }

        private void AddConditionalFlowMarker(Point start, Point end)
        {
            var direction = end - start;
            direction.Normalize();

            var markerPos = start;

            var diamond = new Polygon
            {
                Points = new PointCollection
                {
                    markerPos,
                    new(markerPos.X - 4, markerPos.Y - 8),
                    new(markerPos.X, markerPos.Y - 16),
                    new(markerPos.X + 4, markerPos.Y - 8)
                },
                StrokeThickness = 1.5,
                Stroke = Brushes.Black,
                Fill = Brushes.White
            };

            var angle = Math.Atan2(direction.Y, direction.X) * 180 / Math.PI;
            var rotateTransform = new RotateTransform(angle + 90, markerPos.X, markerPos.Y);
            diamond.RenderTransform = rotateTransform;

            _canvas.Children.Add(diamond);
        }
        
        private void AddArrowhead(Point endPoint, Vector direction)
        {
            var arrow = new Polygon
            {
                Points = new PointCollection
                {
                    endPoint,
                    endPoint - direction * 10 + new Vector(-direction.Y, direction.X) * 5,
                    endPoint - direction * 10 + new Vector(direction.Y, -direction.X) * 5
                },
                Fill = Brushes.Black
            };

            _canvas.Children.Add(arrow);
        }
        
        private void AddOpenArrowhead(Point endPoint, Vector direction)
        {
            var arrow = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Points = new PointCollection
                {
                    endPoint,
                    endPoint - direction * 10 + new Vector(-direction.Y, direction.X) * 5,
                    endPoint - direction * 10 + new Vector(direction.Y, -direction.X) * 5,
                    endPoint
                },
                Fill = Brushes.White
            };

            _canvas.Children.Add(arrow);
        }
        
        private void AddDottedCircleMarker(Point position)
        {
            var marker = new Ellipse
            {
                Width = 6,
                Height = 6,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = [2, 2],
                Fill = Brushes.White
            };

            Canvas.SetLeft(marker, position.X - 3);
            Canvas.SetTop(marker, position.Y - 3);
            _canvas.Children.Add(marker);
        }
        
        private void AddFlowLabel(SequenceFlow flow, IEnumerable<Point> points)
        {
            var labelPos = CalculateLabelPosition(points, 0.3);
            var label = new TextBlock
            {
                Text = flow.Name,
                FontSize = 11,
            };

            Canvas.SetLeft(label, labelPos.X);
            Canvas.SetTop(label, labelPos.Y);
            _canvas.Children.Add(label);
        }
        
        private void AddMessageLabel(MessageFlow flow, IEnumerable<Point> points)
        {
            var labelPos = CalculateLabelPosition(points, 0.3);
            var label = new TextBlock
            {
                Text = flow.MessageRef?.Name ?? flow.Name,
                FontSize = 11,
            };

            Canvas.SetLeft(label, labelPos.X);
            Canvas.SetTop(label, labelPos.Y);
            _canvas.Children.Add(label);
        }
        
        private Point CalculateLabelPosition(IEnumerable<Point> points, double ratio)
        {
            var segment = FindLongestSegment(points);
            return new Point(
                segment.Start.X + (segment.End.X - segment.Start.X) * ratio,
                segment.Start.Y + (segment.End.Y - segment.Start.Y) * ratio
            );
        }
        
        private (Point Start, Point End) FindLongestSegment(IEnumerable<Point> points)
        {
            var pointList = points.ToList();
            if (pointList.Count < 2) 
                return (pointList.First(), pointList.Last());

            double maxDistance = 0;
            int maxIndex = 0;

            for (int i = 0; i < pointList.Count - 1; i++)
            {
                double currentDistance = Helpers.CalculateDistance(
                    pointList[i], 
                    pointList[i + 1]
                );

                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    maxIndex = i;
                }
            }

            return maxDistance > 0 
                ? (pointList[maxIndex], pointList[maxIndex + 1])
                : (pointList.First(), pointList.Last());
        }
    }
}