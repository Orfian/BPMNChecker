using System.Windows;
using System.Windows.Shapes;
using BPMNModel.Model;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class EventSimulator : BaseSimulator
{
    public EventSimulator(ILogger logger, TokenManager tokenManager, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<System.Windows.Point>> paths) : base(logger, tokenManager, objectBounds, paths)
    {
    }

    public override void Evaluate(BPMNToken token, IList<SimulationAction> actions)
    {
        if (token?.CurrentElement is not Event evt)
        {
            base.Evaluate(token, actions);
            return;
        }

        switch (evt)
        {
            case StartEvent start:
                HandleStartEvent(token, start, actions);
                break;

            case EndEvent end:
                HandleEndEvent(token, end, actions);
                break;

            case IntermediateCatchEvent catchEvent:
                HandleIntermediateCatchEvent(token, catchEvent, actions);
                break;

            case IntermediateThrowEvent throwEvent:
                HandleIntermediateThrowEvent(token, throwEvent, actions);
                break;

            default:
                _logger.Warning(
                    "Unhandled event type: {EventType}",
                    evt.GetType().Name);
                break;
        }
    }

    private void HandleStartEvent(BPMNToken token, StartEvent start, IList<SimulationAction> actions)
    {
        base.Evaluate(token, actions);
    }

    private void HandleEndEvent(BPMNToken token, EndEvent end, IList<SimulationAction> actions)
    {
        var parent = token.Parent;

        actions.Add(new RemoveTokenAction(token));
        
        var allTokens = _tokenManager.GetAllTokens();

        if (parent != null)
        {
            var siblings = allTokens
                .Where(t => t.Parent == parent && t != token)
                .ToList();

            if (!siblings.Any())
            {
                actions.Add(new SetTokenWaitingAction(parent, false));
                base.Evaluate(parent, actions);
            }
        }
        
        if (allTokens.Count == 0)
        {
            MessageBox.Show("Simulation completed. No more tokens in the process.", "Simulation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void HandleIntermediateCatchEvent(BPMNToken token, IntermediateCatchEvent catchEvent, IList<SimulationAction> actions)
    {
        var eventDefinition = catchEvent.EventDefinitions.FirstOrDefault();
        
        switch (eventDefinition)
        {
            case MessageEventDefinition messageDef:
                HandleMessageCatchEvent(token, messageDef, actions);
                break;
            case TimerEventDefinition timerDef:
                HandleTimerCatchEvent(token, timerDef, actions);
                break;
            case ConditionalEventDefinition condDef:
                HandleConditionalCatchEvent(token, condDef, actions);
                break;
            case SignalEventDefinition signalDef:
                HandleSignalCatchEvent(token, signalDef, actions);
                break;
            default:
                base.Evaluate(token, actions);
                break;
        }
    }
    
    private void HandleIntermediateThrowEvent(BPMNToken token, IntermediateThrowEvent throwEvent, IList<SimulationAction> actions)
    {
        base.Evaluate(token, actions);
    }
    
    private void HandleMessageStartEvent(BPMNToken token, MessageEventDefinition messageDef, IList<SimulationAction> actions)
    {
        base.Evaluate(token, actions);
    }
    
    private void HandleTimerStartEvent(BPMNToken token, TimerEventDefinition timerDef, IList<SimulationAction> actions)
    {
        base.Evaluate(token, actions);
    }
    
    private void HandleConditionalStartEvent(BPMNToken token, ConditionalEventDefinition condDef, IList<SimulationAction> actions)
    {
        base.Evaluate(token, actions);
    }
    
    private void HandleSignalStartEvent(BPMNToken token, SignalEventDefinition signalDef, IList<SimulationAction> actions)
    {
        base.Evaluate(token, actions);
    }
    
    private void HandleMessageCatchEvent(BPMNToken token, MessageEventDefinition messageDef, IList<SimulationAction> actions)
    {
        if (token.IsWaiting)
            return;
        
        actions.Add(new SetTokenWaitingAction(token, true));
        actions.Add(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    private void HandleTimerCatchEvent(BPMNToken token, TimerEventDefinition timerDef, IList<SimulationAction> actions)
    {
        if (token.IsWaiting)
            return;
        
        actions.Add(new SetTokenWaitingAction(token, true));
        actions.Add(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    private void HandleConditionalCatchEvent(BPMNToken token, ConditionalEventDefinition condDef, IList<SimulationAction> actions)
    {
        if (token.IsWaiting)
            return;
        
        actions.Add(new SetTokenWaitingAction(token, true));
        actions.Add(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    private void HandleSignalCatchEvent(BPMNToken token, SignalEventDefinition signalDef, IList<SimulationAction> actions)
    {
        if (token.IsWaiting)
            return;
        
        actions.Add(new SetTokenWaitingAction(token, true));
        actions.Add(new EventDelayAction(token, token.CurrentElement as Event));
    }
    
    public void ResolveEventDelay(BPMNToken token, Polygon indicator, IList<SimulationAction> actions)
    {
        _tokenManager.RemoveChoiceIndicator(indicator);
        base.Evaluate(token, actions);
    }
    
    public void SpawnStartEventIndicator(StartEvent startEvent, BPMNToken? parentToken, IList<SimulationAction> actions)
    {
        if (_objectBounds.TryGetValue(startEvent.Id, out var bounds))
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