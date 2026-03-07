using System.Windows;
using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public class SimulationActionList
{
    private readonly SharedVariables _vars;
    
    private TokenManager _tokenManager;
    
    private ActivitySimulator _activitySimulator;
    private EventSimulator _eventSimulator;
    private GatewaySimulator _gatewaySimulator;
    
    public readonly Queue<SimulationMessage> MessageQueue = new();
    public readonly Queue<SimulationSignal> SignalQueue = new();
    
    public List<GatewayChoice> PendingGatewayChoices = new();
    public List<ElementTrigger> PendingElementTriggers = new();
    
    public List<string> TriggeredElementTriggers { get; } = new();
    
    public List<SetTokenWaitingAction> SetTokenWaitingActions { get; } = new();
    public List<ResolveGatewayChoiceAction> ResolveGatewayChoiceActions { get; } = new();
    public List<RequestGatewayChoiceAction> RequestGatewayChoiceActions { get; } = new();
    public List<SplitTokenAction> SplitTokenActions { get; } = new();
    public List<MoveTokenAction> MoveTokenActions { get; } = new();
    public List<SpawnTokenAction> SpawnTokenActions { get; } = new();
    public List<SpawnCollapsedSubProcessAction> SpawnCollapsedSubProcessActions { get; } = new();
    public List<RemoveTokenAction> RemoveTokenActions { get; } = new();
    public List<DelayTokenAction> DelayTokenActions { get; } = new();
    public List<SendMessageAction> SendMessageActions { get; } = new();
    public List<SendSignalAction> SendSignalActions { get; } = new();
    
    public SimulationActionList(TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        _vars = SharedVariables.Instance;
        
        _tokenManager = tokenManager;
        _activitySimulator = activitySimulator;
        _eventSimulator = eventSimulator;
        _gatewaySimulator = gatewaySimulator;
    }
    
    public void SetSimulators(ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        _activitySimulator = activitySimulator;
        _eventSimulator = eventSimulator;
        _gatewaySimulator = gatewaySimulator;
    }
    
    public void AddAction(SimulationAction action)
    {
        switch (action)
        {
            case SetTokenWaitingAction a:
                SetTokenWaitingActions.Add(a);
                break;
            case RequestGatewayChoiceAction a:
                RequestGatewayChoiceActions.Add(a);
                break;
            case ResolveGatewayChoiceAction a:
                ResolveGatewayChoiceActions.Add(a);
                break;
            case SplitTokenAction a:
                SplitTokenActions.Add(a);
                break;
            case MoveTokenAction a:
                MoveTokenActions.Add(a);
                break;
            case SpawnTokenAction a:
                SpawnTokenActions.Add(a);
                break;
            case SpawnCollapsedSubProcessAction a:
                SpawnCollapsedSubProcessActions.Add(a);
                break;
            case RemoveTokenAction a:
                RemoveTokenActions.Add(a);
                break;
            case DelayTokenAction a:
                DelayTokenActions.Add(a);
                break;
            case SendMessageAction a:
                SendMessageActions.Add(a);
                break;
            case SendSignalAction a:
                SendSignalActions.Add(a);
                break;
        }
    }
    
    public void CommitActions()
    {
        CommitResolveGatewayChoiceActions();
        CommitRequestGatewayChoiceActions();
        CommitSetTokenWaitingActions();
        CommitSplitTokenActions();
        CommitMoveTokenActions();
        CommitSpawnTokenActions();
        CommitSpawnCollapsedSubProcessActions();
        CommitRemoveTokenActions();
        CommitDelayTokenActions();
        CommitSendMessageActions();
        CommitSendSignalActions();
    }
    
    public void ClearActions()
    {
        SetTokenWaitingActions.Clear();
        RequestGatewayChoiceActions.Clear();
        ResolveGatewayChoiceActions.Clear();
        SplitTokenActions.Clear();
        MoveTokenActions.Clear();
        SpawnTokenActions.Clear();
        SpawnCollapsedSubProcessActions.Clear();
        RemoveTokenActions.Clear();
        DelayTokenActions.Clear();
        SendMessageActions.Clear();
        SendSignalActions.Clear();
    }
    
    public void ClearPendingGatewayChoices()
    {
        foreach (var choice in PendingGatewayChoices)
        {
            var manager = choice.Token?.Owner ?? _tokenManager;
            foreach (var indicator in choice.Indicators)
            {
                if (indicator.Visual != null)
                {
                    manager.RemoveChoiceIndicator(indicator.Visual);
                }
            }
        }
        PendingGatewayChoices.Clear();
    }
    
    public void ClearAll()
    {
        ClearActions();
        ClearPendingGatewayChoices();
        MessageQueue.Clear();
        SignalQueue.Clear();
        PendingElementTriggers.Clear();
        TriggeredElementTriggers.Clear();
    }
    
    public void CommitSetTokenWaitingActions()
    {
        foreach (var a in SetTokenWaitingActions)
        {
            var manager = a.Token.Owner ?? _tokenManager;
            manager.SetTokenWaiting(a.Token, a.IsWaiting);
        }
        SetTokenWaitingActions.Clear();
    }
    
    public void CommitResolveGatewayChoiceActions()
    {
        foreach (var a in ResolveGatewayChoiceActions)
        {
            var choice = a.GatewayChoice;
            var manager = choice.Token?.Owner ?? _tokenManager;
                
            var selectedFlows = choice.Indicators
                .Where(ind => ind.Selected && ind.Flow != null)
                .Select(ind => ind.Flow!)
                .ToList();
                
            switch (choice.Gateway)
            {
                case ParallelGateway pg:
                    _vars.Logger.Warning("Unexpected gateway action: {ActionType}", choice.Gateway.GetType().Name);
                    break;
                case ExclusiveGateway eg:
                    _gatewaySimulator.ResolveExclusiveGateway(choice.Token, eg, selectedFlows);
                    break;
                case InclusiveGateway ig:
                    _gatewaySimulator.ResolveInclusiveGateway(choice.Token, ig, selectedFlows);
                    break;
                case ComplexGateway cg:
                    _gatewaySimulator.ResolveComplexGateway(choice.Token, cg, selectedFlows);
                    break;
                case EventBasedGateway ebg:
                    _gatewaySimulator.ResolveEventBasedGateway(choice.Token, ebg, selectedFlows);
                    break;
                default:
                    _vars.Logger.Warning("Unknown gateway action: {ActionType}", choice.Gateway.GetType().Name);
                    break;
            }

            foreach (var indicator in a.GatewayChoice.Indicators)
            {
                if (indicator.Visual != null)
                {
                    manager.RemoveChoiceIndicator(indicator.Visual);
                }
            }
            
            PendingGatewayChoices.Remove(a.GatewayChoice);
        }
        ResolveGatewayChoiceActions.Clear();
    }

    public void CommitRequestGatewayChoiceActions()
    {
        foreach (var a in RequestGatewayChoiceActions)
        {
            var manager = a.Token.Owner ?? _tokenManager;
            
            var gatewayChoice = new GatewayChoice
            {
                Token = a.Token,
                Gateway = a.Gateway,
                OutgoingFlows = a.OutgoingFlows,
                MultiSelect = a.MultiSelect,
                DefaultFlow = a.DefaultFlow
            };

            manager.ShowGatewayChoiceIndicators(gatewayChoice);
                    
            if (gatewayChoice.DefaultFlow != null)
            {
                var defaultIndicator = gatewayChoice.Indicators
                    .FirstOrDefault(ind => ind.Flow == gatewayChoice.DefaultFlow);
                if (defaultIndicator != null && defaultIndicator.Visual != null)
                {
                    defaultIndicator.Selected = true;
                    manager.SetIndicatorColor(defaultIndicator.Visual, true);
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
                        manager.SetIndicatorColor(ind.Visual, true);
                    }
                }
            }
                    
            PendingGatewayChoices.Add(gatewayChoice);

            ResolveGatewayChoiceActions.Add(new ResolveGatewayChoiceAction(gatewayChoice));
        }
        RequestGatewayChoiceActions.Clear();
    }
    
    public void CommitSplitTokenActions()
    {
        foreach (var a in SplitTokenActions)
        {
            if (_vars.ObjectBounds.TryGetValue(a.SourceElement.Id!, out var sourceBounds))
            {
                var manager = a.ParentToken?.Owner ?? _tokenManager;
                var newToken = manager.AddToken(a.SourceElement, sourceBounds);
                newToken.Parent = a.ParentToken;
                        
                if (_vars.ObjectBounds.TryGetValue(a.TargetElement.Id!, out var targetBounds))
                {
                    manager.MoveToken(
                        newToken,
                        a.TargetElement,
                        a.Flow,
                        _vars.Paths.TryGetValue(a.Flow.Id!, out var path) ? path : null,
                        targetBounds
                    );
                }
            }
        }
        SplitTokenActions.Clear();
    }
    
    public void CommitMoveTokenActions()
    {
        foreach (var a in MoveTokenActions)
        {
            if (_vars.ObjectBounds.TryGetValue(a.TargetElement.Id!, out var targetBounds))
            {
                var manager = a.Token.Owner ?? _tokenManager;
                manager.MoveToken(
                    a.Token,
                    a.TargetElement,
                    a.Flow,
                    _vars.Paths.TryGetValue(a.Flow.Id!, out var path) ? path : null,
                    targetBounds
                );
            }
        }
        MoveTokenActions.Clear();
    }
    
    public void CommitSpawnTokenActions()
    {
        foreach (var a in SpawnTokenActions)
        {
            if (_vars.ObjectBounds.TryGetValue(a.TargetElement.Id!, out var targetBounds))
            {
                var manager = a.ParentToken?.Owner ?? _tokenManager;
                var newToken = manager.AddToken(a.TargetElement, targetBounds);
                newToken.Parent = a.ParentToken;
            }
        }
        SpawnTokenActions.Clear();
    }
    
    public void CommitSpawnCollapsedSubProcessActions()
    {
        var vars = SharedVariables.Instance;

        foreach (var a in SpawnCollapsedSubProcessActions)
        {
            var subDiagram = vars.Model.Definition?.Diagrams
                .FirstOrDefault(d => d.Plane?.BpmnElement?.Id == a.SubProcess.Id);

            if (subDiagram == null) continue;

            // Reuse existing window or create a new one
            if (!vars.SubProcessWindows.TryGetValue(a.SubProcess.Id, out var window))
            {
                window = new SubProcessWindow
                {
                    Owner = Application.Current.MainWindow,
                    Title = $"Sub-Process: {a.SubProcess.Name ?? a.SubProcess.Id}"
                };
                window.RenderSubProcess(subDiagram);
                vars.SubProcessWindows[a.SubProcess.Id] = window;
            }

            window.StartSimulation(a.SubProcess, a.ParentToken);

            if (!window.IsVisible)
                window.Show();
            else
                window.Activate();
        }
        SpawnCollapsedSubProcessActions.Clear();
    }
    
    public void CommitRemoveTokenActions()
    {
        foreach (var a in RemoveTokenActions)
        {
            var manager = a.Token.Owner ?? _tokenManager;
            manager.RemoveToken(a.Token);
        }
        RemoveTokenActions.Clear();
    }
    
    public void CommitDelayTokenActions()
    {
        foreach (var a in DelayTokenActions)
        {
            var manager = a.Token.Owner ?? _tokenManager;
            
            var trigger = new ElementTrigger
            {
                Token = a.Token,
                Element = a.Element
            };
            
            manager.ShowEventTriggerIndicator(trigger);
            
            var arrow = trigger.Indicator.Visual;
            if (arrow == null) continue;
            
            arrow.MouseDown += (s, e) =>
            {
                ResolveElementTrigger(trigger);
            };
            
            PendingElementTriggers.Add(trigger);
        }
        DelayTokenActions.Clear();
    }
    
    public void CommitSendMessageActions()
    {
        foreach (var a in SendMessageActions)
        {
            var message = new SimulationMessage(
                a.MessageName,
                a.Token.CurrentElement?.Id ?? "Unknown"
            );
            MessageQueue.Enqueue(message);
            _vars.Logger.Information("Message {MessageName} sent from {SourceId}", message.MessageName, message.SourceElementId);
        }
        SendMessageActions.Clear();
    }
    
    public void CommitSendSignalActions()
    {
        foreach (var a in SendSignalActions)
        {
            var signal = new SimulationSignal(
                a.SignalName,
                a.Token.CurrentElement?.Id ?? "Unknown"
            );
            SignalQueue.Enqueue(signal);
            _vars.Logger.Information("Signal {SignalName} sent from {SourceId}", signal.SignalName, signal.SourceElementId);
        }
        SendSignalActions.Clear();
    }

    public void ResolveElementTrigger(ElementTrigger trigger)
    {
        var isMessage = false;
        var isSignal = false;
        
        if (trigger.Element is CatchEvent catchEvent)
        {
            isMessage = catchEvent.EventDefinitions.Any(def => def is MessageEventDefinition);
            isSignal = catchEvent.EventDefinitions.Any(def => def is SignalEventDefinition);
        }
        else if (trigger.Element is BoundaryEvent boundaryEvent)
        {
            isMessage = boundaryEvent.EventDefinitions.Any(def => def is MessageEventDefinition);
            isSignal = boundaryEvent.EventDefinitions.Any(def => def is SignalEventDefinition);
        }
        else if (trigger.Element is SendTask sendTask)
        {
            isMessage = true;
        }
        
        if (isMessage)
        {
            var dialog = new QueueDialog("Message Queue", "Trigger Without Message", MessageQueue);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.SelectedItem is SimulationMessage selectedMessage)
                {
                    // Consume message
                    var messageList = MessageQueue.ToList();
                    
                    // Find the specific instance to remove
                    var index = messageList.FindIndex(m => ReferenceEquals(m, selectedMessage));
                    if (index != -1)
                    {
                        messageList.RemoveAt(index);
                    }
                    
                    MessageQueue.Clear();
                    foreach(var m in messageList) MessageQueue.Enqueue(m);
                    
                    _vars.Logger.Information("Message {MessageName} consumed by {ElementId}", selectedMessage.MessageName, trigger.Element.Id);
                    TriggeredElementTriggers.Add($"Message \"{selectedMessage.MessageName}\" consumed by {trigger.Element.Id}");
                    if (trigger.Element is SendTask)
                    {
                        _activitySimulator.ResolveElementDelay(trigger);
                    }
                    else
                    {
                        _eventSimulator.ResolveElementDelay(trigger);
                    }
                }
                else if (dialog.TriggerWithoutSelection)
                {
                    _vars.Logger.Information("Element {ElementId} triggered manually without message", trigger.Element.Id);
                    TriggeredElementTriggers.Add($"Element {trigger.Element.Id} triggered manually without message");
                    _eventSimulator.ResolveElementDelay(trigger);
                }
            }
        }
        else if (isSignal)
        {
            var dialog = new QueueDialog("Signal Queue", "Trigger Without Signal", SignalQueue);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.SelectedItem is SimulationSignal selectedSignal)
                {
                    var signalList = SignalQueue.ToList();
                    var index = signalList.FindIndex(sig => ReferenceEquals(sig, selectedSignal));
                    if (index != -1)
                    {
                        signalList.RemoveAt(index);
                    }
                    
                    SignalQueue.Clear();
                    foreach(var sig in signalList) SignalQueue.Enqueue(sig);
                    
                    _vars.Logger.Information("Signal {SignalName} consumed by {ElementId}", selectedSignal.SignalName, trigger.Element.Id);
                    TriggeredElementTriggers.Add($"Signal \"{selectedSignal.SignalName}\" consumed by {trigger.Element.Id}");
                    _eventSimulator.ResolveElementDelay(trigger);
                }
                else if (dialog.TriggerWithoutSelection)
                {
                    _vars.Logger.Information("Element {ElementId} triggered manually without signal", trigger.Element.Id);
                    TriggeredElementTriggers.Add($"Element {trigger.Element.Id} triggered manually without signal");
                    _eventSimulator.ResolveElementDelay(trigger);
                }
            }
        }
        else
        {
            TriggeredElementTriggers.Add($"Element {trigger.Element.Id} triggered");
            _eventSimulator.ResolveElementDelay(trigger);
        }
        
        PendingElementTriggers.Remove(trigger);
    }
}

