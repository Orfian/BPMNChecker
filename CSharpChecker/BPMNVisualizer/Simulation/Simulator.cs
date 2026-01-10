using System.Windows;
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

    private readonly List<SimulationAction> _actions = new();

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
        var allStartEvents = _model.AllObjectsWithIds.Values
            .OfType<StartEvent>()
            .ToList();

        var subProcessStartEventIds = _model.AllObjectsWithIds.Values
            .OfType<SubProcess>()
            .SelectMany(sp => sp.FlowElements.OfType<StartEvent>())
            .Select(se => se.Id)
            .ToHashSet();

        var startEvents = allStartEvents
            .Where(se => !subProcessStartEventIds.Contains(se.Id))
            .Where(se => !IsMessageOrSignalStart(se))
            .ToList();

        if (!startEvents.Any())
        {
            startEvents = allStartEvents
                .Where(se => !subProcessStartEventIds.Contains(se.Id))
                .ToList();
        }

        if (!startEvents.Any())
        {
            startEvents = allStartEvents;
        }

        if (!startEvents.Any())
        {
            _logger.Warning("No start events found in BPMN model.");
            return;
        }

        foreach (var startEvent in startEvents)
        {
            if (_objectBounds.TryGetValue(startEvent.Id, out var bounds))
            {
                _tokenManager.AddToken(startEvent, bounds);
            }
        }
    }
    
    public void NextStep()
    {
        _actions.Clear();

        var tokens = _tokenManager.GetAllTokens().ToList();
        foreach (var token in tokens)
        {
            token.IsEvaluated = false;
        }

        foreach (var token in tokens)
        {
            if (token.IsEvaluated)
                continue;
            
            var simulator = GetElementSimulator(token.CurrentElement);
            simulator.Evaluate(token, _actions);
            token.IsEvaluated = true;
        }

        CommitActions();
    }
    
    private void CommitActions()
    {
        foreach (var action in _actions)
        {
            switch (action)
            {
                case MoveTokenAction move:
                {
                    if (_objectBounds.TryGetValue(move.TargetElement.Id, out var bounds))
                    {
                        _tokenManager.MoveToken(
                            move.Token,
                            move.TargetElement,
                            move.Flow,
                            _paths.TryGetValue(move.Flow.Id, out var path) ? path : null,
                            bounds
                        );
                    }
                    break;
                }

                case SplitTokenAction split:
                {
                    if (_objectBounds.TryGetValue(split.SourceElement.Id, out var sourceBounds))
                    {
                        var newToken = _tokenManager.AddToken(split.SourceElement, sourceBounds);
                        newToken.Parent = split.ParentToken;
                        
                        if (_objectBounds.TryGetValue(split.TargetElement.Id, out var targetBounds))
                        {
                            _tokenManager.MoveToken(
                                newToken,
                                split.TargetElement,
                                split.Flow,
                                _paths.TryGetValue(split.Flow.Id, out var path) ? path : null,
                                targetBounds
                            );
                        }
                    }
                    break;
                }

                case SpawnTokenAction spawn:
                {
                    if (_objectBounds.TryGetValue(spawn.TargetElement.Id, out var bounds))
                    {
                        var newToken = _tokenManager.AddToken(spawn.TargetElement, bounds);
                        newToken.Parent = spawn.ParentToken;
                    }
                    break;
                }

                case RemoveTokenAction remove:
                    _tokenManager.RemoveToken(remove.Token);
                    break;

                case SetTokenWaitingAction wait:
                    _tokenManager.SetTokenWaiting(wait.Token, wait.IsWaiting);
                    break;
                
                default:
                    _logger.Warning("Unknown simulation action: {ActionType}", action.GetType().Name);
                    break;
            }
        }
    }
    
    private IElementSimulator GetElementSimulator(BaseElement element)
    {
        return element switch
        {
            Activity => _activitySimulator,
            Event => _eventSimulator,
            Gateway => _gatewaySimulator,
            _ => throw new NotSupportedException(
                $"No simulator available for element type: {element.GetType().Name}")
        };
    }

    private bool IsMessageOrSignalStart(StartEvent startEvent)
    {
        if (startEvent?.EventDefinitions == null || !startEvent.EventDefinitions.Any())
            return false;

        var def = startEvent.EventDefinitions.First();
        return def is MessageEventDefinition || def is SignalEventDefinition;
    }
}
