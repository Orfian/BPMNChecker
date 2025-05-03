using System.Windows;
using System.Windows.Controls;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;
using Task = BPMNModel.Model.Task;

namespace BPMNVisualizer.Visualization.Renderers;

public class ActivityRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;

    public ActivityRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager, SvgResourceManager svgResourceManager)
    {
        _logger = logger;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
        _svgResourceManager = svgResourceManager;
    }

    public void RenderShape(BaseElement element, Rect bounds)
    {
        if (element is not Activity activity) return;

        var shape = DrawElement(activity, bounds);
        Canvas.SetLeft(shape, bounds.Left);
        Canvas.SetTop(shape, bounds.Top);
        _canvas.Children.Add(shape);
        
        if (activity is Task task)
        {
            var icon = DrawIcon(task, bounds);
            if (icon != null)
            {
                Canvas.SetLeft(icon, bounds.Left + bounds.Width * 0.05);
                Canvas.SetTop(icon, bounds.Top + bounds.Height * 0.05);
                _canvas.Children.Add(icon);
            }
        }
        
        var markers = DrawMarkers(activity, bounds);
        if (markers != null)
        {
            Canvas.SetLeft(markers, bounds.Left + (bounds.Width - markers.Width) / 2);
            Canvas.SetTop(markers, bounds.Top + bounds.Height * 0.9 - markers.Height);
            _canvas.Children.Add(markers);
        }
        
        var label = DrawLabel(activity, bounds);
        if (label != null)
        {
            Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
            Canvas.SetTop(label, bounds.Top + (bounds.Height - label.Height) / 2);
            _canvas.Children.Add(label);
        }
    }

    private UIElement DrawElement(Activity activity, Rect bounds)
    {
        var shape = activity switch
        {
            Task => _shapeManager.GetRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            CallActivity => 
                _shapeManager.GetThickRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            Transaction => 
                _shapeManager.GetDoubleRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            SubProcess { TriggeredByEvent: true } =>
                _shapeManager.GetRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity), true),
            SubProcess =>
                _shapeManager.GetRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            _ => throw new NotSupportedException($"Activity type {activity.GetType()} is not supported.")
        };
        return shape;
    }
    
    private Border? DrawIcon(Task task, Rect bounds)
    {
        var icon = _svgResourceManager.GetTaskIcon(task);
        if (icon == null)
        {
            _logger.Warning("No icon found for task type: {TaskType}", task.GetType());
            return null;
        }
        return _shapeManager.WrapInContainer(icon, bounds, 0.40);
    }
    
    private Border? DrawMarkers(Activity activity, Rect bounds)
    {
        var markers = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
        };

        if (activity.IsForCompensation == true || HasCompensationEvent(activity))
        {
            AddMarker("Compensation", markers, bounds);
        }
        
        if (activity.LoopCharacteristics != null)
        {
            switch (activity.LoopCharacteristics)
            {
                case MultiInstanceLoopCharacteristics mi:
                    var markerType = mi.IsSequential == true ? "Sequential-MultiInstance" : "Parallel-MultiInstance";
                    AddMarker(markerType, markers, bounds);
                    break;
                    
                case StandardLoopCharacteristics:
                    AddMarker("Loop", markers, bounds);
                    break;
            }
        }

        if (activity is SubProcess) AddMarker("SubProcess", markers, bounds);
        if (activity is AdHocSubProcess) AddMarker("AdHoc", markers, bounds);

        return markers.Children.Count > 0 ? _shapeManager.WrapInContainer(markers, bounds) : null;
    }

    private bool HasCompensationEvent(Activity activity)
    {
        return activity.BoundaryEventRefs.Any(e =>
            e.EventDefinitions.Any(d => d is CompensateEventDefinition));
    }

    private void AddMarker(string markerType, StackPanel container, Rect bounds)
    {
        var marker = _svgResourceManager.GetMarker(markerType);
        if (marker == null)
        {
            _logger.Warning("Marker icon not found: {MarkerType}", markerType);
            return;
        }

        double size = Math.Min(bounds.Width, bounds.Height) * 0.2;
        var viewbox = new Viewbox
        {
            Child = marker,
            Width = size,
            Height = size
        };

        container.Children.Add(viewbox);
    }
    
    private Border? DrawLabel(Activity activity, Rect bounds)
    {
        var text = activity.Name;
        if (string.IsNullOrEmpty(text))
        {
            _logger.Warning("No label found for activity type: {ActivityType}", activity.GetType());
            return null;
        }
        
        bounds.Width *= 0.9;
        
        var label = _shapeManager.GetLabel(text, bounds);
        label.VerticalAlignment = VerticalAlignment.Center;
        
        
        return _shapeManager.WrapInContainer(label, bounds);
    }
}