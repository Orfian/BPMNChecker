using System.Windows;
using BPMNVisualizer.Utility;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class BaseSimulator : IElementSimulator
{
    protected readonly TokenManager _tokenManager;
    protected readonly SimulationActionList _actions;
    
    public BaseSimulator(TokenManager tokenManager, SimulationActionList actions)
    {
        _tokenManager = tokenManager;
        _actions = actions;
    }

    public virtual void Evaluate(BPMNToken token)
    {
        if (token?.CurrentElement == null)
        {
            SharedVariables.Instance.Logger.Warning("Token or its current element is null.");
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

            _actions.AddAction(new SetTokenWaitingAction(token, false));
            _actions.AddAction(new MoveTokenAction(token, target, flow));
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
                _actions.AddAction(new SetTokenWaitingAction(token, false));
                _actions.AddAction(new MoveTokenAction(token, target, flow));
                first = false;
            }
            else
            {
                _actions.AddAction(new SplitTokenAction(token.CurrentElement, target, flow, token.Parent));
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