using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel;
using Serilog;


namespace BPMNVisualizer.Visualization
{
public class Visualizer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly RendererFactory _rendererFactory;

    public Visualizer(ILogger logger, Canvas canvas)
    {
        _logger = logger;
        _canvas = canvas;
        _rendererFactory = new RendererFactory(_logger, _canvas);
    }

    public void Visualize(ModelRoot model)
    {
        _canvas.Children.Clear();
        if (model.Definition?.Diagrams.FirstOrDefault()?.Plane == null) return;

        var visualizationPipeline = new VisualizationPipeline(_logger, _canvas, _rendererFactory);
        visualizationPipeline.Execute(model);
    }
}
}