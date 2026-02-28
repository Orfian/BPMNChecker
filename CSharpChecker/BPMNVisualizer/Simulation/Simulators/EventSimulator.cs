using System.Windows;
using System.Windows.Shapes;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class EventSimulator : BaseSimulator
{
    public EventSimulator(TokenManager tokenManager, SimulationActionList actions)
        : base(tokenManager, actions)
    {
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
                SharedVariables.Instance.Logger.Warning(
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
    
    private void HandleMessageStartEvent(BPMNToken token, MessageEventDefinition messageDef)
    {
        base.Evaluate(token);
    }
    
    private void HandleTimerStartEvent(BPMNToken token, TimerEventDefinition timerDef)
    {
        base.Evaluate(token);
    }
    
    private void HandleConditionalStartEvent(BPMNToken token, ConditionalEventDefinition condDef)
    {
        base.Evaluate(token);
    }
    
    private void HandleSignalStartEvent(BPMNToken token, SignalEventDefinition signalDef)
    {
        base.Evaluate(token);
    }
    
    private void HandleMessageCatchEvent(BPMNToken token, MessageEventDefinition messageDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    private void HandleTimerCatchEvent(BPMNToken token, TimerEventDefinition timerDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    private void HandleConditionalCatchEvent(BPMNToken token, ConditionalEventDefinition condDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    private void HandleSignalCatchEvent(BPMNToken token, SignalEventDefinition signalDef)
    {
        if (token.IsWaiting)
            return;
        
        _actions.AddAction(new SetTokenWaitingAction(token, true));
        _actions.AddAction(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    public void ResolveEventDelay(BPMNToken token, Polygon indicator)
    {
        _tokenManager.RemoveChoiceIndicator(indicator);
        base.Evaluate(token);
    }
    
    public void SpawnStartEventIndicator(StartEvent startEvent, BPMNToken? parentToken)
    {
        if (SharedVariables.Instance.ObjectBounds.TryGetValue(startEvent.Id, out var bounds))
        {
            var arrow = _tokenManager.AddArrowIndicator(
                new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2),
                new Vector(1, 0));
                    
            arrow.MouseDown += (s, e) =>
            {
                var token = _tokenManager.AddToken(startEvent, bounds);
                token.Parent = parentToken;
                _tokenManager.RemoveChoiceIndicator(arrow);
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