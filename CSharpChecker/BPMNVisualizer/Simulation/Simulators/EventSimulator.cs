using System.Windows;
using System.Windows.Shapes;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class EventSimulator : BaseSimulator
{
    private readonly SharedVariables _vars;
    
    public EventSimulator(TokenManager tokenManager, SimulationActionList actions)
        : base(tokenManager, actions)
    {
        _vars = SharedVariables.Instance;
    }

    public override void Evaluate(BPMNToken token)
    {
        if (token?.CurrentElement is not Event evt)
        {
            base.Evaluate(token);
            return;
        }

        switch (evt)
        {
            case StartEvent start:
                HandleStartEvent(token, start);
                break;

            case EndEvent end:
                HandleEndEvent(token, end);
                break;

            case IntermediateCatchEvent catchEvent:
                HandleIntermediateCatchEvent(token, catchEvent);
                break;

            case IntermediateThrowEvent throwEvent:
                HandleIntermediateThrowEvent(token, throwEvent);
                break;

            default:
                _vars.Logger.Warning(
                    "Unhandled event type: {EventType}",
                    evt.GetType().Name);
                break;
        }
    }

    private void HandleStartEvent(BPMNToken token, StartEvent start)
    {
        base.Evaluate(token);
    }

    private void HandleEndEvent(BPMNToken token, EndEvent end)
    {
        var messageDef = end.EventDefinitions.OfType<MessageEventDefinition>().FirstOrDefault();
        if (messageDef != null)
        {
            var messageName = messageDef.MessageRef?.Name ?? end.Name ?? end.Id ?? "UnknownMessage";
            _actions.AddAction(new SendMessageAction(token, messageName));
        }

        var signalDef = end.EventDefinitions.OfType<SignalEventDefinition>().FirstOrDefault();
        if (signalDef != null)
        {
            var signalName = signalDef.SignalRef?.Name ?? end.Name ?? end.Id ?? "UnknownSignal";
            _actions.AddAction(new SendSignalAction(token, signalName));
        }

        var parent = token.Parent;

        _actions.AddAction(new RemoveTokenAction(token));
        
        var allTokens = _tokenManager.GetAllTokens();
        
        var terminateDef = end.EventDefinitions.OfType<TerminateEventDefinition>().FirstOrDefault();
        if (terminateDef != null)
        {
            var tokensToRemove = parent == null
                ? allTokens
                : allTokens.Where(t => t.Parent == parent).ToList();

            foreach (var t in tokensToRemove)
            {
                _actions.AddAction(new RemoveTokenAction(t));
            }
        }

        if (parent != null)
        {
            var siblings = allTokens
                .Where(t => t.Parent == parent && t != token)
                .ToList();

            if (!siblings.Any())
            {
                _actions.AddAction(new SetTokenWaitingAction(parent, false));
                base.Evaluate(parent);
            }
        }
        
        if (allTokens.Count == 0)
        {
            MessageBox.Show("Simulation completed. No more tokens in the process.", "Simulation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void HandleIntermediateCatchEvent(BPMNToken token, IntermediateCatchEvent catchEvent)
    {
        var eventDefinition = catchEvent.EventDefinitions.FirstOrDefault();
        
        switch (eventDefinition)
        {
            case MessageEventDefinition messageDef:
                HandleMessageCatchEvent(token, messageDef);
                break;
            case TimerEventDefinition timerDef:
                HandleTimerCatchEvent(token, timerDef);
                break;
            case ConditionalEventDefinition condDef:
                HandleConditionalCatchEvent(token, condDef);
                break;
            case SignalEventDefinition signalDef:
                HandleSignalCatchEvent(token, signalDef);
                break;
            case LinkEventDefinition linkDef:
                HandleLinkCatchEvent(token, linkDef);
                break;
            default:
                base.Evaluate(token);
                break;
        }
    }
    
    private void HandleIntermediateThrowEvent(BPMNToken token, IntermediateThrowEvent throwEvent)
    {
        var messageDef = throwEvent.EventDefinitions.OfType<MessageEventDefinition>().FirstOrDefault();
        if (messageDef != null)
        {
            var messageName = messageDef.MessageRef?.Name ?? throwEvent.Name ?? throwEvent.Id ?? "UnknownMessage";
            _actions.AddAction(new SendMessageAction(token, messageName));
        }

        var signalDef = throwEvent.EventDefinitions.OfType<SignalEventDefinition>().FirstOrDefault();
        if (signalDef != null)
        {
            var signalName = signalDef.SignalRef?.Name ?? throwEvent.Name ?? throwEvent.Id ?? "UnknownSignal";
            _actions.AddAction(new SendSignalAction(token, signalName));
        }

        base.Evaluate(token);
    }
    
    private void HandleMessageCatchEvent(BPMNToken token, MessageEventDefinition messageDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new DelayTokenAction(token, token.CurrentElement));
    }
    
    private void HandleTimerCatchEvent(BPMNToken token, TimerEventDefinition timerDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new DelayTokenAction(token, token.CurrentElement));
    }
    
    private void HandleConditionalCatchEvent(BPMNToken token, ConditionalEventDefinition condDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new DelayTokenAction(token, token.CurrentElement));
    }
    
    private void HandleSignalCatchEvent(BPMNToken token, SignalEventDefinition signalDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new DelayTokenAction(token, token.CurrentElement));
    }
    
    private void HandleLinkCatchEvent(BPMNToken token, LinkEventDefinition linkDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new DelayTokenAction(token, token.CurrentElement));
    }
    
    public void ResolveElementDelay(ElementTrigger trigger)
    {
        _tokenManager.RemoveArrowIndicator(trigger.Indicator.Visual);
        base.Evaluate(trigger.Token);
    }
    
    public void SpawnStartEventIndicator(StartEvent startEvent, BPMNToken? parentToken)
    {
        if (_vars.ObjectBounds.TryGetValue(startEvent.Id, out var bounds))
        {
            var arrow = _tokenManager.AddArrowIndicator(
                new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2),
                new Vector(1, 0));
                    
            arrow.MouseDown += (s, e) =>
            {
                if (startEvent.EventDefinitions.Any(def => def is MessageEventDefinition))
                {
                    var dialog = new QueueDialog("Message Queue", "Trigger Without Message", _actions.MessageQueue);
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.SelectedItem is SimulationMessage selectedMessage)
                        {
                            // Consume message
                            var messageList = _actions.MessageQueue.ToList();
                            
                            // Find the specific instance to remove
                            var index = messageList.FindIndex(m => ReferenceEquals(m, selectedMessage));
                            if (index != -1)
                            {
                                messageList.RemoveAt(index);
                            }
                            
                            _actions.MessageQueue.Clear();
                            foreach(var m in messageList) _actions.MessageQueue.Enqueue(m);
                            
                            _vars.Logger.Information("Message {MessageName} consumed by {EventId}", selectedMessage.MessageName, startEvent.Id);
                            _actions.TriggeredElementTriggers.Add($"Message \"{selectedMessage.MessageName}\" consumed by Start Event {startEvent.Id}");
                            
                            var token = _tokenManager.AddToken(startEvent, bounds);
                            token.Parent = parentToken;
                        }
                        else if (dialog.TriggerWithoutSelection)
                        {
                            _vars.Logger.Information("Event {EventId} triggered manually without message", startEvent.Id);
                            _actions.TriggeredElementTriggers.Add($"Message Start Event {startEvent.Id} triggered manually");
                            
                            var token = _tokenManager.AddToken(startEvent, bounds);
                            token.Parent = parentToken;
                        }
                    }
                }
                else if (startEvent.EventDefinitions.Any(def => def is SignalEventDefinition))
                {
                    var dialog = new QueueDialog("Signal Queue", "Trigger Without Signal", _actions.SignalQueue);
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.SelectedItem is SimulationSignal selectedSignal)
                        {
                            var signalList = _actions.SignalQueue.ToList();
                            var index = signalList.FindIndex(sig => ReferenceEquals(sig, selectedSignal));
                            if (index != -1)
                            {
                                signalList.RemoveAt(index);
                            }
                            
                            _actions.SignalQueue.Clear();
                            foreach(var sig in signalList) _actions.SignalQueue.Enqueue(sig);
                            
                            _vars.Logger.Information("Signal {SignalName} consumed by {EventId}", selectedSignal.SignalName, startEvent.Id);
                            _actions.TriggeredElementTriggers.Add($"Signal \"{selectedSignal.SignalName}\" consumed by Start Event {startEvent.Id}");
                            
                            var token = _tokenManager.AddToken(startEvent, bounds);
                            token.Parent = parentToken;
                        }
                        else if (dialog.TriggerWithoutSelection)
                        {
                            _vars.Logger.Information("Event {EventId} triggered manually without signal", startEvent.Id);
                            _actions.TriggeredElementTriggers.Add($"Signal Start Event {startEvent.Id} triggered manually");
                            
                            var token = _tokenManager.AddToken(startEvent, bounds);
                            token.Parent = parentToken;
                        }
                    }
                }
                else
                {
                    _actions.TriggeredElementTriggers.Add($"Start Event {startEvent.Id} triggered");
                    var token = _tokenManager.AddToken(startEvent, bounds);
                    token.Parent = parentToken;
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
        }
    }
}
/*
using System.Windows;
using BPMNModel;
using BPMNModel.Model;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class EventSimulator : BaseSimulator
{
    public EventSimulator(ILogger logger, TokenManager tokenManager, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
        : base(logger, tokenManager, objectBounds, paths)
    {
    }

    public override void OnTokenArrived(BPMNToken token)
    {
        if (token?.CurrentElement == null)
        {
            _logger.Warning("Token or its current element is null in EventSimulator.");
            return;
        }
        
        if (token.CurrentElement is not Event evt)
        {
            base.OnTokenArrived(token);
            return;
        }
        
        switch (evt)
        {
            case StartEvent start:
                HandleStartEvent(token, start);
                break;
            case EndEvent end:
                HandleEndEvent(token);
                break;
            case IntermediateCatchEvent catchEvent:
                HandleIntermediateCatchEvent(token, catchEvent);
                break;
            case IntermediateThrowEvent throwEvent:
                HandleIntermediateThrowEvent(token, throwEvent);
                break;
            default:
                _logger.Warning("Unhandled event type: {EventType}", evt.GetType().Name);
                break;
        }
    }
    
    private void HandleStartEvent(BPMNToken token, StartEvent start)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleEndEvent(BPMNToken token)
    {
        var parent = token.Parent;
        _tokenManager.RemoveToken(token);
        
        if (parent != null)
        {
            var otherTokensInSubProcess = _tokenManager.GetAllTokens()
                .Where(t => t.Parent == parent)
                .ToList();
            if (!otherTokensInSubProcess.Any())
            {
                _tokenManager.SetTokenWaiting(parent, false);
                base.OnTokenArrived(parent);
            }
        }
        
        if (_tokenManager.GetAllTokens().Count == 0)
        {
            MessageBox.Show("Simulation completed. No more tokens in the process.", "Simulation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void HandleIntermediateCatchEvent(BPMNToken token, IntermediateCatchEvent catchEvent)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleIntermediateThrowEvent(BPMNToken token, IntermediateThrowEvent throwEvent)
    {
        base.OnTokenArrived(token);
    }
}
*/