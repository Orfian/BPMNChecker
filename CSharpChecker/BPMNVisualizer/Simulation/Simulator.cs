using System.Windows;
using System.Windows.Controls;
using BPMNModel;
using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation;

public class Simulator
{
    private readonly TokenManager _tokenManager;
    private readonly ILogger _logger;
    private readonly Dictionary<string, Rect> _objectBounds;
    private readonly Dictionary<string, IEnumerable<Point>> _paths;
    private readonly ModelRoot _model;
    private readonly Canvas _canvas;
    
    private readonly ActivitySimulator _activitySimulator;
    private readonly EventSimulator _eventSimulator;
    private readonly GatewaySimulator _gatewaySimulator;
    
    public Simulator(ILogger logger, TokenManager tokenManager, ModelRoot model, Canvas canvas, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
    {
        _logger = logger;
        _tokenManager = tokenManager;
        _model = model;
        _canvas = canvas;
        _objectBounds = objectBounds;
        _paths = paths;
        
        _activitySimulator = new ActivitySimulator(_logger, _tokenManager, _model, _objectBounds, _paths);
        _eventSimulator = new EventSimulator(_logger, _tokenManager, _model, _objectBounds, _paths);
        _gatewaySimulator = new GatewaySimulator(_logger, _tokenManager, _model, _objectBounds, _paths);
    }
    
    public void NextStep_Click(object sender, RoutedEventArgs e)
    {
        var tokens = _tokenManager.GetAllTokens();
        foreach (var token in tokens)
        {
            var simulator = GetElementSimulator(token.CurrentElement);
            simulator.OnTokenArrived(token);
        }
    }

    public void RegisterStartEvents()
    {
        
    }
    
    private IElementSimulator GetElementSimulator(BaseElement element)
    {
        switch (element)
        {
            case Activity:
                return _activitySimulator;
            case Event:
                return _eventSimulator;
            case Gateway:
                return _gatewaySimulator;
            default:
                throw new NotSupportedException($"No simulator available for element type: {element.GetType().Name}");
        }
    }
}