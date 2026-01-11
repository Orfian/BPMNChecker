using System.Windows;
using BPMNModel;
using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;
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
    private readonly List<SimulationAction> _priorityActions = new();
    private List<GatewayChoice> _pendingChoices = new();

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
            .Where(se => !Helpers.IsMessageOrSignalStart(se))
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

        CommitActions(_priorityActions);
        _priorityActions.Clear();
        
        CommitActions(_actions);
        _actions.Clear();
    }
    
    private void CommitActions(IList<SimulationAction> actions)
    {
        foreach (var action in actions)
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
                
                case RequestGatewayChoiceAction request:
                    var gatewayChoice = new GatewayChoice
                    {
                        Token = request.Token,
                        Gateway = request.Gateway,
                        OutgoingFlows = request.OutgoingFlows,
                        MultiSelect = request.MultiSelect,
                        DefaultFlow = request.DefaultFlow
                    };
                    
                    foreach (var flow in request.OutgoingFlows)
                    {
                        var points = _paths.TryGetValue(flow.Id, out var path) ? path : null;
                        if (points != null && points.Count() >= 2)
                        {
                            var first = points.First();
                            var second = points.Skip(1).FirstOrDefault();
                            var direction = Helpers.GetDirection(first, second);
                            var triangle = _tokenManager.AddChoiceIndicator(first, direction);
                            
                            var indicator = new Indicator
                            {
                                Visual = triangle,
                                Flow = flow,
                                Selected = false
                            };
                            
                            gatewayChoice.Indicators.Add(indicator);
                            
                            triangle.MouseDown += (s, e) =>
                            {
                                _gatewaySimulator.UpdatePendingChoices(indicator, gatewayChoice);
                            };
                            
                            triangle.MouseEnter += (s, e) =>
                            {
                                _tokenManager.SetHoverIndicatorColor(triangle, indicator.Selected, true);
                            };
                            
                            triangle.MouseLeave += (s, e) =>
                            {
                                _tokenManager.SetHoverIndicatorColor(triangle, indicator.Selected, false);
                            };
                        }
                    }
                    
                    if (gatewayChoice.DefaultFlow != null)
                    {
                        var defaultIndicator = gatewayChoice.Indicators
                            .FirstOrDefault(ind => ind.Flow == gatewayChoice.DefaultFlow);
                        if (defaultIndicator != null)
                        {
                            defaultIndicator.Selected = true;
                            _tokenManager.SetIndicatorColor(defaultIndicator.Visual, true);
                        }
                    }
                    else
                    {
                        if (gatewayChoice.Gateway is not ComplexGateway && gatewayChoice.Gateway is not EventBasedGateway)
                        {
                            var ind = gatewayChoice.Indicators.First();
                            ind.Selected = true;
                            _tokenManager.SetIndicatorColor(ind.Visual, true);
                        }
                    }
                    
                    _pendingChoices.Add(gatewayChoice);

                    _priorityActions.Add(new ResolveGatewayChoiceAction(gatewayChoice));
                    break;
                
                case ResolveGatewayChoiceAction resolve:
                    var choice = resolve.GatewayChoice;
                    
                    var selectedFlows = choice.Indicators
                        .Where(ind => ind.Selected)
                        .Select(ind => ind.Flow)
                        .ToList();
                    
                    switch (choice.Gateway)
                    {
                        case ParallelGateway pg:
                            _logger.Warning("Unexpected gateway action: {ActionType}", choice.Gateway.GetType().Name);
                            break;
                        case ExclusiveGateway eg:
                            _gatewaySimulator.ResolveExclusiveGateway(choice.Token, eg, selectedFlows, _actions);
                            break;
                        case InclusiveGateway ig:
                            _gatewaySimulator.ResolveInclusiveGateway(choice.Token, ig, selectedFlows, _actions);
                            break;
                        case ComplexGateway cg:
                            _gatewaySimulator.ResolveComplexGateway(choice.Token, cg, selectedFlows, _actions);
                            break;
                        case EventBasedGateway ebg:
                            _gatewaySimulator.ResolveEventBasedGateway(choice.Token, ebg, selectedFlows, _actions);
                            break;
                        default:
                            _logger.Warning("Unknown gateway action: {ActionType}", choice.Gateway.GetType().Name);
                            break;
                    }

                    foreach (var indicator in resolve.GatewayChoice.Indicators)
                    {
                        _tokenManager.RemoveChoiceIndicator(indicator.Visual);
                    }
                    _pendingChoices.Remove(resolve.GatewayChoice);
                    break;
                
                case EventDelayAction requestDelay:
                    var eventPosition = _objectBounds.TryGetValue(requestDelay.Event.Id, out var evtBounds)
                        ? new Point(evtBounds.X + evtBounds.Width / 2, evtBounds.Y + evtBounds.Height / 2)
                        : new Point(0, 0);
                    
                    var arrow= _tokenManager.AddChoiceIndicator(eventPosition, new Vector(1, 0));
                    
                    arrow.MouseDown += (s, e) =>
                    {
                        _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                    };
                    
                    arrow.MouseEnter += (s, e) =>
                    {
                        _tokenManager.SetHoverIndicatorColor(arrow, false, true);
                    };
                            
                    arrow.MouseLeave += (s, e) =>
                    {
                        _tokenManager.SetHoverIndicatorColor(arrow, false, false);
                    };
                    
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
    
    public void ClearAllActions()
    {
        _priorityActions.Clear();
        _actions.Clear();
    }
    
    public void ClearPendingChoices()
    {
        foreach (var choice in _pendingChoices)
        {
            foreach (var indicator in choice.Indicators)
            {
                _tokenManager.RemoveChoiceIndicator(indicator.Visual);
            }
        }
        _pendingChoices.Clear();
    }
}
