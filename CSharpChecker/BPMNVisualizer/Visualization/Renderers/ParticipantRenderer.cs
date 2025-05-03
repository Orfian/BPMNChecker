using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Visualization.Renderers;

public class ParticipantRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;

    public ParticipantRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager)
    {
        _logger = logger;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
    }

    public void RenderShape(BaseElement element, Rect bounds)
    {
        switch (element)
        {
            case Participant participant:
                RenderParticipant(participant, bounds);
                break;
            case Lane lane:
                RenderLane(lane, bounds);
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
}