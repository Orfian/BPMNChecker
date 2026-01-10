using System.Windows;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class BaseSimulator : IElementSimulator
{
    protected readonly ILogger _logger;
    protected readonly TokenManager _tokenManager;
    protected readonly Dictionary<string, Rect> _objectBounds;
    protected readonly Dictionary<string, IEnumerable<Point>> _paths;
    
    public BaseSimulator(ILogger logger, TokenManager tokenManager, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
    {
        _logger = logger;
        _tokenManager = tokenManager;
        _objectBounds = objectBounds;
        _paths = paths;
    }

    public virtual void Evaluate(BPMNToken token, IList<SimulationAction> actions)
    {
        if (token?.CurrentElement == null)
        {
            _logger.Warning("Token or its current element is null.");
            return;
        }

        var outgoingFlows = _tokenManager
            .GetOutgoingFlows(token.CurrentElement)
            .ToList();

        if (!outgoingFlows.Any())
            return;

        if (outgoingFlows.Count == 1)
        {
            var flow = outgoingFlows[0];
            var target = _tokenManager.GetTargetElement(flow);
            if (target == null)
                return;

            actions.Add(new SetTokenWaitingAction(token, false));
            actions.Add(new MoveTokenAction(token, target, flow));
            return;
        }

        bool first = true;

        foreach (var flow in outgoingFlows)
        {
            var target = _tokenManager.GetTargetElement(flow);
            if (target == null)
                continue;

            if (first)
            {
                actions.Add(new SetTokenWaitingAction(token, false));
                actions.Add(new MoveTokenAction(token, target, flow));
                first = false;
            }
            else
            {
                actions.Add(new SplitTokenAction(token.CurrentElement, target, flow, token.Parent));
            }
        }
    }
    
    protected string? ShowChoiceDialog(string title, string message, List<string> options)
    {
        var dialog = new ChoiceDialog(title, message, options, multiSelect: false);
        if (dialog.ShowDialog() == true)
            return dialog.SelectedOptions.FirstOrDefault();
        return null;
    }

    protected List<string> ShowMultiChoiceDialog(string title, string message, List<string> options, string defaultOption = "")
    {
        var dialog = new ChoiceDialog(title, message, options, multiSelect: true, defaultOption);
        if (dialog.ShowDialog() == true)
            return dialog.SelectedOptions;
        return new List<string>();
    }
}

/*
    public virtual void OnTokenArrived(BPMNToken token)
    {
        if (token == null || token.CurrentElement == null)
        {
            _logger.Warning("Token or its current element is null.");
            return;
        }

        var outgoingFlows = _tokenManager.GetOutgoingFlows(token.CurrentElement);

        bool splitting = outgoingFlows.Count() > 1;
        if (!splitting)
        {
            var flow = outgoingFlows.First();
            var target = _tokenManager.GetTargetElement(flow);
            if (target == null) return;

            var bounds = _objectBounds[target.Id];
            var path = _paths.ContainsKey(flow.Id) ? _paths[flow.Id] : null;

            _tokenManager.SetTokenWaiting(token, false);
            _tokenManager.MoveToken(token, target, flow, path, bounds);

            return;
        }

        var originalElement = token.CurrentElement;

        bool first = true;
        foreach (var flow in outgoingFlows)
        {
            var target = _tokenManager.GetTargetElement(flow);
            if (target == null) continue;

            var bounds = _objectBounds[target.Id];
            var path = _paths.ContainsKey(flow.Id) ? _paths[flow.Id] : null;

            if (first)
            {
                _tokenManager.SetTokenWaiting(token, false);
                _tokenManager.MoveToken(token, target, flow, path, bounds);
                first = false;
            }
            else
            {
                var newToken = _tokenManager.AddToken(originalElement, bounds);
                newToken.Parent = token.Parent;
                _tokenManager.MoveToken(newToken, target, flow, path, bounds);
            }
        }
    }
*/