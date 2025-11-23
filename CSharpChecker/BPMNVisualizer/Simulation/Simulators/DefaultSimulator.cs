using System.Windows;
using BPMNModel;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class DefaultSimulator : IElementSimulator
{
    protected readonly ILogger _logger;
    protected readonly TokenManager _tokenManager;
    protected readonly Dictionary<string, Rect> _objectBounds;
    protected readonly Dictionary<string, IEnumerable<Point>> _paths;
    protected readonly ModelRoot _model;
    
    public DefaultSimulator(ILogger logger, TokenManager tokenManager, ModelRoot model, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
    {
        _logger = logger;
        _tokenManager = tokenManager;
        _model = model;
        _objectBounds = objectBounds;
        _paths = paths;
    }

    public virtual void OnTokenArrived(BPMNToken token)
    {
        if (token == null || token.CurrentElement == null)
        {
            _logger.Warning("Token or its current element is null.");
            return;
        }
        
        var outgoingFlows = _tokenManager.GetOutgoingFlows(_model, token.CurrentElement);
        
        bool splitting = outgoingFlows.Count() > 1;
        if (!splitting)
        {
            var flow = outgoingFlows.First();
            var target = _tokenManager.GetTargetElement(_model, flow);
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
            var target = _tokenManager.GetTargetElement(_model, flow);
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
                _tokenManager.MoveToken(newToken, target, flow, path, bounds);
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
        /*
        var dialog = new ChoiceDialog(title, message, options, multiSelect: true);
        if (dialog.ShowDialog() == true)
            return dialog.SelectedOptions;
        return new List<string>();
        */
    }
}