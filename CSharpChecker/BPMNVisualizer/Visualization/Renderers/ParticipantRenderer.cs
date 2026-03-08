using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Serilog;

namespace BPMNVisualizer.Visualization.Renderers;

public class ParticipantRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    
    private BPMNShape _shape;

    public ParticipantRenderer(Canvas canvas, BrushManager brushManager, ShapeManager shapeManager)
    {
        _logger = SharedVariables.Instance.Logger;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
    }

    public void RenderShape(BPMNShape shape)
    {
        _shape = shape;
        var element = shape.BpmnElement;
        var bounds = new Rect(shape?.Bounds?.X ?? 0, shape?.Bounds?.Y ?? 0, shape?.Bounds?.Width ?? 0, shape?.Bounds?.Height ?? 0);
        switch (element)
        {
            case Participant participant:
                RenderParticipant(participant, bounds);
                break;
            case Lane lane:
                RenderLane(lane, bounds);
                break;
            case Group group:
                RenderGroup(group, bounds);
                break;
            case TextAnnotation textAnnotation:
                RenderTextAnnotation(textAnnotation, bounds);
                break;
            default:
                _logger.Warning("Unsupported participant type: {ElementType}", element.GetType());
                break;
        }
    }

    private void RenderParticipant(Participant participant, Rect bounds)
    {
        var poolContainer = _shapeManager.GetRectangle(bounds);

        Canvas.SetLeft(poolContainer, bounds.Left);
        Canvas.SetTop(poolContainer, bounds.Top);
        _canvas.Children.Add(poolContainer);
        
        var label = DrawLabel(participant, bounds);
        if (label != null)
        {
            label.RenderTransform = new RotateTransform(-90);
            Canvas.SetLeft(label, bounds.Left + label.Height / 3);
            Canvas.SetTop(label, bounds.Top + (bounds.Height + label.Width) / 2);
            _canvas.Children.Add(label);
        }
    }

    private void RenderLane(Lane lane, Rect bounds)
    {
        if (bounds.IsEmpty)
        {
            _logger.Warning("Empty bounds for lane: {LaneId}", lane.Id);
            return;
        }

        var laneBackground = _shapeManager.GetRectangle(bounds);

        Canvas.SetLeft(laneBackground, bounds.Left);
        Canvas.SetTop(laneBackground, bounds.Top);
        _canvas.Children.Add(laneBackground);

        var label = DrawLabel(lane, bounds);
        if (label != null)
        {
            label.RenderTransform = new RotateTransform(-90);
            Canvas.SetLeft(label, bounds.Left + label.Height / 3);
            Canvas.SetTop(label, bounds.Top + (bounds.Height + label.Width) / 2);
            _canvas.Children.Add(label);
        }
    }
    
    private Border? DrawLabel(Participant participant, Rect bounds)
    {
        var text = participant.Name;
        if (string.IsNullOrEmpty(text))
        {
            _logger.Warning("No label found for participant type: {ParticipantType}", participant.GetType());
            return null;
        }
        
        var label = _shapeManager.GetLabel(text, new Rect(bounds.Left, bounds.Top, bounds.Height, 20));
        
        return _shapeManager.WrapInContainer(label, new Rect(bounds.Left, bounds.Top, bounds.Height, 20));
    }
    
    private Border? DrawLabel(Lane lane, Rect bounds)
    {
        var text = lane.Name;
        if (string.IsNullOrEmpty(text))
        {
            _logger.Warning("No label found for lane type: {LaneType}", lane.GetType());
            return null;
        }
        
        var label = _shapeManager.GetLabel(text, new Rect(bounds.Left, bounds.Top, bounds.Height, 20));
        
        return _shapeManager.WrapInContainer(label, new Rect(bounds.Left, bounds.Top, bounds.Height, 20));
    }

    private void RenderGroup(Group group, Rect bounds)
    {
        // Dashed rounded rectangle — no fill, just a border
        var rect = new Rectangle
        {
            Width = bounds.Width,
            Height = bounds.Height,
            Stroke = Brushes.DarkGray,
            StrokeThickness = 2,
            StrokeDashArray = new DoubleCollection([7.5, 2.4, 1, 2.4]),
            RadiusX = 8,
            RadiusY = 8,
            Fill = Brushes.Transparent
        };
        Canvas.SetLeft(rect, bounds.Left);
        Canvas.SetTop(rect, bounds.Top);
        _canvas.Children.Add(rect);

        // Label from the CategoryValue name (top-left corner)
        var labelText = group.CategoryValueRef?.Value;
        if (!string.IsNullOrEmpty(labelText))
        {
            var label = new TextBlock
            {
                Text = labelText,
                FontSize = 11,
                Foreground = Brushes.Gray,
                Background = Brushes.White,
                Padding = new Thickness(2, 0, 2, 0)
            };
            Canvas.SetLeft(label, bounds.Left + 8);
            Canvas.SetTop(label, bounds.Top - 8);
            _canvas.Children.Add(label);
        }
    }

    private void RenderTextAnnotation(TextAnnotation annotation, Rect bounds)
    {
        // Open bracket shape: left vertical line + top + bottom horizontal ticks
        var bracket = new Polyline
        {
            Stroke = Brushes.Black,
            StrokeThickness = 1.5,
            Points = new PointCollection
            {
                new(bounds.Left + 12, bounds.Top),
                new(bounds.Left, bounds.Top),
                new(bounds.Left, bounds.Bottom),
                new(bounds.Left + 12, bounds.Bottom)
            }
        };
        Canvas.SetLeft(bracket, 0);
        Canvas.SetTop(bracket, 0);
        _canvas.Children.Add(bracket);

        // Text label inside
        if (!string.IsNullOrEmpty(annotation.Text))
        {
            var label = new TextBlock
            {
                Text = annotation.Text,
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
                Width = bounds.Width - 16,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(label, bounds.Left + 16);
            Canvas.SetTop(label, bounds.Top + 2);
            _canvas.Children.Add(label);
        }
    }
}