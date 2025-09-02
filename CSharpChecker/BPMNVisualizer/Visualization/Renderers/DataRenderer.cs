using System.Windows;
using System.Windows.Controls;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;
using DataObject = BPMNModel.Model.DataObject;

namespace BPMNVisualizer.Visualization.Renderers;

public class DataRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;

    public DataRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager, SvgResourceManager svgResourceManager)
    {
        _logger = logger;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
        _svgResourceManager = svgResourceManager;
    }

    public void RenderShape(BaseElement element, Rect bounds)
    {
        var (dataElement, dataLabel) = element switch
        {
            DataInput input => ("DataInput", input.Name),
            DataOutput output => ("DataOutput", output.Name),
            DataStoreReference store => ("DataStore", store.Name),
            DataObject obj => ("DataObject", obj.Name),
            _ => (null, null)
        };
            
        if (dataElement == null) return;
        
        var shape = DrawElement(dataElement, bounds);
        if (shape == null) return;
        Canvas.SetLeft(shape, bounds.Left);
        Canvas.SetTop(shape, bounds.Top);
        _canvas.Children.Add(shape);
        
        var label = DrawLabel(dataLabel, bounds);
        if (label != null)
        {
            Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
            Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
            _canvas.Children.Add(label);
        }
    }
    
    private Border? DrawElement(string dataElement, Rect bounds)
    {
        var icon = _svgResourceManager.GetDataIcon(dataElement);
        if (icon == null)
        {
            _logger.Warning("No icon found for data element type: {DataElementType}", dataElement);
            return null;
        }
        return _shapeManager.WrapInContainer(icon, bounds);
    }
    
    private Border? DrawLabel(string? dataLabel, Rect bounds)
    {
        var text = dataLabel;
        if (string.IsNullOrEmpty(text))
        {
            _logger.Warning("No label found for data element type: {DataElementType}", dataLabel);
            return null;
        }
        
        bounds.Width *= 1.5;
        var label = _shapeManager.GetLabel(text, bounds);
        
        return _shapeManager.WrapInContainer(label, bounds);
    }
}