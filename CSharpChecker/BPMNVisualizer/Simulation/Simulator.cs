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
    
    private readonly ActivitySimulator _activitySimulator;
    private readonly EventSimulator _eventSimulator;
    private readonly GatewaySimulator _gatewaySimulator;
    
    public Simulator(ILogger logger, TokenManager tokenManager, ModelRoot model, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
    {
        _logger = logger;
        _tokenManager = tokenManager;
        _model = model;
        _objectBounds = objectBounds;
        _paths = paths;
        
        _activitySimulator = new ActivitySimulator(_logger, _tokenManager, _objectBounds, _paths);
        _eventSimulator = new EventSimulator(_logger, _tokenManager, _objectBounds, _paths);
        _gatewaySimulator = new GatewaySimulator(_logger, _tokenManager, _objectBounds, _paths);
    }

    public void FirstStep()
    {
        var allStartEvents = _model.AllObjectsWithIds.Values.OfType<StartEvent>().ToList();
        var subProcessStartEventIds = _model.AllObjectsWithIds.Values
            .OfType<SubProcess>()
            .SelectMany(subProcess => subProcess.FlowElements.OfType<StartEvent>())
            .Select(startEvent => startEvent.Id)
            .ToHashSet();
        
        var startEvents = allStartEvents.Where(startEvent => !subProcessStartEventIds.Contains(startEvent.Id))
            .Where(startEvent => !IsMessageOrSignalStart(startEvent))
            .ToList();
        
        if (!startEvents.Any())
        {
            startEvents = allStartEvents.Where(startEvent => !subProcessStartEventIds.Contains(startEvent.Id)).ToList();
            
            if (!startEvents.Any())
            {
                startEvents = allStartEvents.ToList();
                
                if (!startEvents.Any())
                {
                    _logger.Warning("No start events found in the BPMN model.");
                    return;
                }
            }
        }
        
        foreach (var startEvent in startEvents)
        {
            _tokenManager.AddToken(startEvent, _objectBounds[startEvent.Id]);
        }
    }
    
    public void NextStep()
    {
        var tokens = _tokenManager.GetAllTokens();
        foreach (var token in tokens)
        {
            StepToken(token);
        }
    }
    
    public void StepToken(BPMNToken token)
    {
        var simulator = GetElementSimulator(token.CurrentElement);
        simulator.OnTokenArrived(token);
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
    
    private bool IsMessageOrSignalStart(StartEvent startEvent)
    {
        if (startEvent?.EventDefinitions == null || !startEvent.EventDefinitions.Any()) return false;
        var def = startEvent.EventDefinitions.First();
        return def is MessageEventDefinition || def is SignalEventDefinition;
    }
}