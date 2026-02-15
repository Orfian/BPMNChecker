using System.Windows;
using System.Collections.ObjectModel;
using BPMNModel;
using BPMNModel.Model;
using BPMNModel.Camunda;
using BPMNVisualizer.Simulation.History;
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
    private readonly Queue<SimulationMessage> _messageQueue = new();
    private readonly Queue<SimulationSignal> _signalQueue = new();
    
    public readonly ObservableCollection<SimulationState> History = new();
    private int _currentStepIndex = -1;

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
        
        var userStartEvents = allStartEvents
            .Where(Helpers.IsUserStart)
            .Where(se => !subProcessStartEventIds.Contains(se.Id))
            .ToList();

        var startEvents = allStartEvents
            .Where(se => !subProcessStartEventIds.Contains(se.Id))
            .Where(se => !Helpers.IsUserStart(se))
            .ToList();

        if (!startEvents.Any())
        {
            _logger.Warning("No start events found in BPMN model.");
            return;
        }

        foreach (var startEvent in startEvents)
        {
            if (_objectBounds.TryGetValue(startEvent.Id!, out var bounds))
            {
                _tokenManager.AddToken(startEvent, bounds);
            }
        }
        
        if (userStartEvents.Any())
        {
            foreach (var startEvent in userStartEvents)
            {
                _eventSimulator.SpawnStartEventIndicator(startEvent, null, _actions);
            }
        }
        
        SaveState(_tokenManager.GetAllTokens());
    }
    
    public void NextStep()
    {
        var tokens = _tokenManager.GetAllTokens().ToList();
        if (!tokens.Any())
        {
            MessageBox.Show("No more tokens.", "Next Step not allowed", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (_currentStepIndex >= 0 && _currentStepIndex < History.Count)
        {
            var oldState = History[_currentStepIndex];
            oldState.PendingGatewayChoices = _pendingChoices.Select(c => c.DeepClone()).ToList();
        }
        
        foreach (var token in tokens)
        {
            token.IsEvaluated = false;
        }

        foreach (var token in tokens)
        {
            if (token.IsEvaluated || token.CurrentElement == null)
                continue;
            
            var simulator = GetElementSimulator(token.CurrentElement);
            simulator.Evaluate(token, _actions);
            token.IsEvaluated = true;
        }

        CommitActions(_priorityActions);
        _priorityActions.Clear();
        
        CommitActions(_actions);
        _actions.Clear();
        
        SaveState(_tokenManager.GetAllTokens());
    }
    
    private void CommitActions(IList<SimulationAction> actions)
    {
        foreach (var action in actions)
        {
            switch (action)
            {
                case MoveTokenAction move:
                {
                    if (_objectBounds.TryGetValue(move.TargetElement.Id!, out var bounds))
                    {
                        _tokenManager.MoveToken(
                            move.Token,
                            move.TargetElement,
                            move.Flow,
                            _paths.TryGetValue(move.Flow.Id!, out var path) ? path : null,
                            bounds
                        );
                    }
                    break;
                }

                case SplitTokenAction split:
                {
                    if (_objectBounds.TryGetValue(split.SourceElement.Id!, out var sourceBounds))
                    {
                        var newToken = _tokenManager.AddToken(split.SourceElement, sourceBounds);
                        newToken.Parent = split.ParentToken;
                        
                        if (_objectBounds.TryGetValue(split.TargetElement.Id!, out var targetBounds))
                        {
                            _tokenManager.MoveToken(
                                newToken,
                                split.TargetElement,
                                split.Flow,
                                _paths.TryGetValue(split.Flow.Id!, out var path) ? path : null,
                                targetBounds
                            );
                        }
                    }
                    break;
                }

                case SpawnTokenAction spawn:
                {
                    if (_objectBounds.TryGetValue(spawn.TargetElement.Id!, out var bounds))
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
                    
                    ShowGatewayChoiceIndicators(gatewayChoice);
                    
                    if (gatewayChoice.DefaultFlow != null)
                    {
                        var defaultIndicator = gatewayChoice.Indicators
                            .FirstOrDefault(ind => ind.Flow == gatewayChoice.DefaultFlow);
                        if (defaultIndicator != null && defaultIndicator.Visual != null)
                        {
                            defaultIndicator.Selected = true;
                            _tokenManager.SetIndicatorColor(defaultIndicator.Visual, true);
                        }
                    }
                    else
                    {
                        if (gatewayChoice.Gateway is not ComplexGateway && gatewayChoice.Gateway is not EventBasedGateway)
                        {
                            var ind = gatewayChoice.Indicators.FirstOrDefault();
                            if (ind != null && ind.Visual != null)
                            {
                                ind.Selected = true;
                                _tokenManager.SetIndicatorColor(ind.Visual, true);
                            }
                        }
                    }
                    
                    _pendingChoices.Add(gatewayChoice);

                    _priorityActions.Add(new ResolveGatewayChoiceAction(gatewayChoice));
                    break;
                
                case ResolveGatewayChoiceAction resolve:
                    var choice = resolve.GatewayChoice;
                    
                    var selectedFlows = choice.Indicators
                        .Where(ind => ind.Selected && ind.Flow != null)
                        .Select(ind => ind.Flow!)
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
                        if (indicator.Visual != null)
                        {
                            _tokenManager.RemoveChoiceIndicator(indicator.Visual);
                        }
                    }
                    _pendingChoices.Remove(resolve.GatewayChoice);
                    break;
                
                case EventDelayAction requestDelay:
                    var eventPosition = _objectBounds.TryGetValue(requestDelay.Event.Id!, out var evtBounds)
                        ? new Point(evtBounds.X + evtBounds.Width / 2, evtBounds.Y + evtBounds.Height / 2)
                        : new Point(0, 0);
                    
                    var arrow= _tokenManager.AddArrowIndicator(eventPosition, new Vector(1, 0));
                    
                    arrow.MouseDown += (s, e) =>
                    {
                        if (requestDelay.Event is StartEvent or EndEvent)
                        {
                             _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                             return;
                        }

                        var isMessageEvent = false;
                        var isSignalEvent = false;
                        if (requestDelay.Event is CatchEvent catchEvent)
                        {
                            isMessageEvent = catchEvent.EventDefinitions.Any(def => def is MessageEventDefinition);
                            isSignalEvent = catchEvent.EventDefinitions.Any(def => def is SignalEventDefinition);
                        }
                        else if (requestDelay.Event is ThrowEvent throwEvent)
                        {
                            isMessageEvent = throwEvent.EventDefinitions.Any(def => def is MessageEventDefinition);
                            isSignalEvent = throwEvent.EventDefinitions.Any(def => def is SignalEventDefinition);
                        }
                        
                        if (isMessageEvent)
                        {
                            var dialog = new QueueDialog("Message Queue", "Trigger Without Message", _messageQueue);
                            if (dialog.ShowDialog() == true)
                            {
                                if (dialog.SelectedItem is SimulationMessage selectedMessage)
                                {
                                    // Consume message
                                    var messageList = _messageQueue.ToList();
                                    
                                    // Find the specific instance to remove
                                    var index = messageList.FindIndex(m => ReferenceEquals(m, selectedMessage));
                                    if (index != -1)
                                    {
                                        messageList.RemoveAt(index);
                                    }
                                    
                                    _messageQueue.Clear();
                                    foreach(var m in messageList) _messageQueue.Enqueue(m);
                                    
                                    _logger.Information("Message {MessageName} consumed by {EventId}", selectedMessage.MessageName, requestDelay.Event.Id);
                                    _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                                }
                                else if (dialog.TriggerWithoutSelection)
                                {
                                    _logger.Information("Event {EventId} triggered manually without message", requestDelay.Event.Id);
                                    _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                                }
                            }
                        }
                        else if (isSignalEvent)
                        {
                            var dialog = new QueueDialog("Signal Queue", "Trigger Without Signal", _signalQueue);
                            if (dialog.ShowDialog() == true)
                            {
                                if (dialog.SelectedItem is SimulationSignal selectedSignal)
                                {
                                    var signalList = _signalQueue.ToList();
                                    var index = signalList.FindIndex(sig => ReferenceEquals(sig, selectedSignal));
                                    if (index != -1)
                                    {
                                        signalList.RemoveAt(index);
                                    }
                                    
                                    _signalQueue.Clear();
                                    foreach(var sig in signalList) _signalQueue.Enqueue(sig);
                                    
                                    _logger.Information("Signal {SignalName} consumed by {EventId}", selectedSignal.SignalName, requestDelay.Event.Id);
                                    _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                                }
                                else if (dialog.TriggerWithoutSelection)
                                {
                                    _logger.Information("Event {EventId} triggered manually without signal", requestDelay.Event.Id);
                                    _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                                }
                            }
                        }
                        else
                        {
                            _eventSimulator.ResolveEventDelay(requestDelay.Token, arrow, _actions);
                        }
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

                case SendMessageAction sendMessage:
                    var message = new SimulationMessage(
                        sendMessage.MessageName,
                        sendMessage.Token.CurrentElement?.Id ?? "Unknown"
                    );
                    _messageQueue.Enqueue(message);
                    _logger.Information("Message {MessageName} sent from {SourceId}", message.MessageName, message.SourceElementId);

                    break;

                case SendSignalAction sendSignal:
                    var signal = new SimulationSignal(
                        sendSignal.SignalName,
                        sendSignal.Token.CurrentElement?.Id ?? "Unknown"
                    );
                    _signalQueue.Enqueue(signal);
                    _logger.Information("Signal {SignalName} sent from {SourceId}", signal.SignalName, signal.SourceElementId);

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
                if (indicator.Visual != null)
                {
                    _tokenManager.RemoveChoiceIndicator(indicator.Visual);
                }
            }
        }
        _pendingChoices.Clear();
    }

    public void SaveState(IEnumerable<BPMNToken> tokens)
    {
        // If we are saving state from a point in history (not the end), truncate future history
        if (_currentStepIndex != -1 && _currentStepIndex < History.Count - 1)
        {
            while (History.Count > _currentStepIndex + 1)
            {
                History.RemoveAt(History.Count - 1);
            }
        }

        var state = new SimulationState
        {
            StepIndex = History.Count,
            Tokens = tokens.Select(t => t.DeepClone()).ToList(),
            PendingGatewayChoices = _pendingChoices.Select(c => c.DeepClone()).ToList(),
            MessageQueue = new Queue<SimulationMessage>(_messageQueue.Select(m => m.DeepClone())),
            SignalQueue = new Queue<SimulationSignal>(_signalQueue.Select(s => s.DeepClone())),
            TriggeredCodeElements = GetTriggeredCodeElements(tokens)
        };
        
        History.Add(state);
        _currentStepIndex = History.Count - 1;
    }
    
    private List<string> GetTriggeredCodeElements(IEnumerable<BPMNToken> tokens)
    {
        var elements = new HashSet<string>();

        // 1. Check Sequence Flows (Edges) that were traversed in this step
        // We look at MoveTokenAction and SplitTokenAction in _actions
        foreach (var action in _actions)
        {
            SequenceFlow? flow = null;
            if (action is MoveTokenAction move) flow = move.Flow;
            else if (action is SplitTokenAction split) flow = split.Flow;

            if (flow != null && flow.ConditionExpression is FormalExpression expr && !string.IsNullOrWhiteSpace(expr.Body?.Value))
            {
                elements.Add($"Flow: {flow.Id} (Condition)");
            }
        }

        // 2. Check Elements (Nodes) where tokens are currently located IF they were evaluated/active this step
        // Note: tokens passed to SaveState are the current state tokens.
        foreach (var token in tokens)
        {
            // If token is at an element, check if that element has code
            if (token.CurrentElement is BaseElement el)
            {
                // Basic check for Script Tasks
                if (el is ScriptTask st && !string.IsNullOrEmpty(st.Script))
                {
                    elements.Add($"Script Task: {st.Name ?? st.Id}");
                }
                // Service Tasks with Camunda Expression
                else if (el is ServiceTask srv && !string.IsNullOrEmpty(srv.Camunda_expression))
                {
                    elements.Add($"Service Task: {srv.Name ?? srv.Id} (Expression)");
                }
                
                // Camunda Execution Listeners
                if (el.CamundaElements?.OfType<CamundaExecutionListener>() is IEnumerable<CamundaExecutionListener> listeners && 
                    listeners.Any(l => l.Script != null || !string.IsNullOrEmpty(l.Expression) || !string.IsNullOrEmpty(l.DelegateExpression)))
                {
                     elements.Add($"Element: {el.Id} (Listeners)");
                }
            }
        }

        return elements.OrderBy(x => x).ToList();
    }
    
    public void ClearHistory()
    {
        History.Clear();
        _currentStepIndex = -1;
    }

    public void LoadState(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= History.Count)
            return;
            
        var state = History[stepIndex];
        _currentStepIndex = stepIndex;

        // 1. Clear current state
        _tokenManager.ClearAllTokens();
        ClearPendingChoices();
        _actions.Clear();
        _priorityActions.Clear();
        _messageQueue.Clear();
        _signalQueue.Clear();
        
        // 2. Restore Queues
        foreach(var m in state.MessageQueue) _messageQueue.Enqueue(m.DeepClone());
        foreach(var s in state.SignalQueue) _signalQueue.Enqueue(s.DeepClone());
        
        // 3. Restore Tokens
        var oldToNew = new Dictionary<string, BPMNToken>();
        
        foreach (var historyToken in state.Tokens)
        {
             if (historyToken.CurrentElement != null && historyToken.Id != null && _objectBounds.TryGetValue(historyToken.CurrentElement.Id!, out var bounds))
             {
                 var newToken = _tokenManager.AddToken(historyToken.CurrentElement, bounds);
                 newToken.Id = historyToken.Id; // Restore ID
                 newToken.IsWaiting = historyToken.IsWaiting;
                 newToken.IsEvaluated = historyToken.IsEvaluated;
                 newToken.CurrentSequenceFlow = historyToken.CurrentSequenceFlow;
                 
                  if (historyToken.CurrentSequenceFlow?.Id != null && _paths.TryGetValue(historyToken.CurrentSequenceFlow.Id, out var path) && path != null) {
                     var points = path.ToList();
                     if (points.Any()) {
                         var lastPoint = points.Last();
                         _tokenManager.SetTokenPosition(newToken, lastPoint);
                     }
                 }

                 _tokenManager.SetTokenWaiting(newToken, newToken.IsWaiting);
                 
                 oldToNew[historyToken.Id] = newToken;
             }
        }
        
        // Fix parents
        foreach (var historyToken in state.Tokens)
        {
            if (historyToken.Parent != null && historyToken.Parent.Id != null && oldToNew.TryGetValue(historyToken.Parent.Id, out var newParent))
            {
                // We need to look up the NEW token corresponding to historyToken
                if (historyToken.Id != null && oldToNew.TryGetValue(historyToken.Id, out var newToken))
                {
                    newToken.Parent = newParent;
                }
            }
        }

        // 4. Restore Pending Gateway Choices
        var newPendingChoices = state.PendingGatewayChoices.Select(c => c.DeepClone()).ToList();
        
        foreach (var choice in newPendingChoices)
        {
             // Fix token reference
             if (choice.Token?.Id != null && oldToNew.TryGetValue(choice.Token.Id, out var newToken))
             {
                 choice.Token = newToken;
             }

             var preservedSelections = choice.Indicators.Where(i => i.Selected && i.Flow?.Id != null).Select(i => i.Flow!.Id).ToHashSet();
             choice.Indicators.Clear();
             
             // Recreate indicators (visuals + objects)
             ShowGatewayChoiceIndicators(choice);
             
             // Restore selection
             foreach (var ind in choice.Indicators)
             {
                 if (ind.Flow?.Id != null && preservedSelections.Contains(ind.Flow.Id) && ind.Visual != null)
                 {
                     ind.Selected = true;
                     _tokenManager.SetIndicatorColor(ind.Visual, true);
                 }
             }
             
             _pendingChoices.Add(choice);
             
             bool actionFound = false;
             for (int i = 0; i < _priorityActions.Count; i++)
             {
                 if (_priorityActions[i] is ResolveGatewayChoiceAction action)
                 {
                     if (action.GatewayChoice.Token?.Id != null && action.GatewayChoice.Gateway?.Id != null && choice.Token?.Id != null && choice.Gateway?.Id != null &&
                         action.GatewayChoice.Token.Id == choice.Token.Id && action.GatewayChoice.Gateway.Id == choice.Gateway.Id)
                     {
                         _priorityActions[i] = action with { GatewayChoice = choice };
                         actionFound = true;
                     }
                 }
             }
             
             if (!actionFound)
             {
                 _priorityActions.Add(new ResolveGatewayChoiceAction(choice));
             }
        }
    }

    private void ShowGatewayChoiceIndicators(GatewayChoice gatewayChoice)
    {
        if (gatewayChoice.OutgoingFlows == null) return;

        foreach (var flow in gatewayChoice.OutgoingFlows)
        {
            var points = flow.Id != null && _paths.TryGetValue(flow.Id, out var path) ? path : null;
            if (points != null && points.Count() >= 2)
            {
                var first = points.First();
                var second = points.Skip(1).FirstOrDefault();
                var direction = Helpers.GetDirection(first, second);
                var triangle = _tokenManager.AddArrowIndicator(first, direction);
                
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
    }
}
