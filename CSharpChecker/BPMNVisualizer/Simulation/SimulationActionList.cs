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
        CommitSendMessageActions();
        CommitSendSignalActions();
        CommitSetTokenWaitingActions();
        CommitSplitTokenActions();
        CommitMoveTokenActions();
        CommitSpawnTokenActions();
        CommitSpawnCollapsedSubProcessActions();
        CommitRemoveTokenActions();
        CommitDelayTokenActions();
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
                    manager.RemoveArrowIndicator(indicator.Visual);
                }
            }
        }
        PendingGatewayChoices.Clear();
    }
    
    public void ClearPendingElementTriggers()
    {
        foreach (var trigger in PendingElementTriggers)
        {
            var manager = trigger.Token?.Owner ?? _tokenManager;
            if (trigger.Indicator.Visual != null)
            {
                manager.RemoveArrowIndicator(trigger.Indicator.Visual);
            }
        }
        PendingElementTriggers.Clear();
    }
    
    public void ClearAll()
    {
        ClearActions();
        ClearPendingGatewayChoices();
        ClearPendingElementTriggers();
        MessageQueue.Clear();
        SignalQueue.Clear();
        TriggeredElementTriggers.Clear();
    }
    
    public void CommitSetTokenWaitingActions()
    {
        foreach (var a in SetTokenWaitingActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        SetTokenWaitingActions.Clear();
    }
    
    public void CommitResolveGatewayChoiceActions()
    {
        foreach (var a in ResolveGatewayChoiceActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        ResolveGatewayChoiceActions.Clear();
    }

    public void CommitRequestGatewayChoiceActions()
    {
        foreach (var a in RequestGatewayChoiceActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        RequestGatewayChoiceActions.Clear();
    }
    
    public void CommitSplitTokenActions()
    {
        foreach (var a in SplitTokenActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        SplitTokenActions.Clear();
    }
    
    public void CommitMoveTokenActions()
    {
        foreach (var a in MoveTokenActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        MoveTokenActions.Clear();
    }
    
    public void CommitSpawnTokenActions()
    {
        foreach (var a in SpawnTokenActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        SpawnTokenActions.Clear();
    }
    
    public void CommitSpawnCollapsedSubProcessActions()
    {
        foreach (var a in SpawnCollapsedSubProcessActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        SpawnCollapsedSubProcessActions.Clear();
    }
    
    public void CommitRemoveTokenActions()
    {
        foreach (var a in RemoveTokenActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        RemoveTokenActions.Clear();
    }
    
    public void CommitDelayTokenActions()
    {
        foreach (var a in DelayTokenActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        DelayTokenActions.Clear();
    }
    
    public void CommitSendMessageActions()
    {
        foreach (var a in SendMessageActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
        }
        SendMessageActions.Clear();
    }
    
    public void CommitSendSignalActions()
    {
        foreach (var a in SendSignalActions)
        {
            a.Commit(this, _tokenManager, _activitySimulator, _eventSimulator, _gatewaySimulator);
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

    public void CheckBoundaryEvents(BPMNToken token)
    {
        var element = token.CurrentElement;
        if (element == null || element is not Activity activity)
            return;

        foreach (var boundaryEvent in activity.BoundaryEventRefs)
        {
            var manager = token.Owner ?? _tokenManager;
            
            var trigger = new ElementTrigger
            {
                Element = boundaryEvent
            };
            
            manager.ShowEventTriggerIndicator(trigger);
            
            var arrow = trigger.Indicator.Visual;
            if (arrow == null) continue;
            
            arrow.MouseDown += (s, e) =>
            {
                trigger.Token = _tokenManager.AddToken(boundaryEvent, _vars.ObjectBounds[boundaryEvent.Id!]);
                ResolveElementTrigger(trigger);
            };
            
            PendingElementTriggers.Add(trigger);
        }
    }
}
