using System.Windows;
using BPMNModel;
using BPMNModel.Model;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class EventSimulator : DefaultSimulator
{
    public EventSimulator(ILogger logger, TokenManager tokenManager, ModelRoot model, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
        : base(logger, tokenManager, model, objectBounds, paths)
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