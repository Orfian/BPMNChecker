using System.Windows;
using BPMNModel.Model;
using Serilog;

namespace BPMNVisualizer.Simulation.Simulators;

public class ActivitySimulator : BaseSimulator
{
    public ActivitySimulator(ILogger logger, TokenManager tokenManager, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<System.Windows.Point>> paths, SimulationActionList actions)
        : base(logger, tokenManager, objectBounds, paths, actions)
    {
    }

    public override void Evaluate(BPMNToken token)
    {
        if (token?.CurrentElement is not Activity activity)
        {
            base.Evaluate(token);
            return;
        }

        switch (activity)
        {
            case SubProcess subProcess:
                HandleSubProcess(token, subProcess);
                break;

            case CallActivity callActivity:
                HandleCallActivity(token, callActivity);
                break;

            case SendTask sendTask:
                HandleSendTask(token, sendTask);
                break;

            default:
                base.Evaluate(token);
                break;
        }
    }

    private void HandleSubProcess(BPMNToken token, SubProcess subProcess)
    {
        if (token.IsWaiting)
            return;

        var startEvents = subProcess.FlowElements
            .OfType<StartEvent>()
            .ToList();

        if (!startEvents.Any())
        {
            base.Evaluate(token);
            return;
        }

        _actions.AddAction(new SetTokenWaitingAction(token, true));

        foreach (var startEvent in startEvents)
        {
            _actions.AddAction(new SpawnTokenAction(startEvent, token));
        }
    }
    
    private void HandleCallActivity(BPMNToken token, CallActivity callActivity)
    {
        base.Evaluate(token);
    }

    private void HandleSendTask(BPMNToken token, SendTask sendTask)
    {
        string messageName = sendTask.MessageRef?.Name ?? sendTask.Name ?? sendTask.Id ?? "UnknownMessage";
        _actions.AddAction(new SendMessageAction(token, messageName));
        base.Evaluate(token);
    }
}

/*
using System.Windows;
using BPMNModel.Model;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation.Simulators;

public class ActivitySimulator : BaseSimulator
{
    public ActivitySimulator(ILogger logger, TokenManager tokenManager, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
        : base(logger, tokenManager, objectBounds, paths)
    {
    }
    
    public override void OnTokenArrived(BPMNToken token)
    {
        if (token?.CurrentElement == null)
        {
            _logger.Warning("Token or its current element is null in ActivitySimulator.");
            return;
        }
        
        if (token.CurrentElement is not Activity activity)
        {
            base.OnTokenArrived(token);
            return;
        }

        switch (activity)
        {
            case SendTask sendTask:
                HandleSendTask(token, sendTask);
                break;
            case ReceiveTask receiveTask:
                HandleReceiveTask(token, receiveTask);
                break;
            case UserTask userTask:
                HandleUserTask(token, userTask);
                break;
            case ManualTask manualTask:
                HandleManualTask(token, manualTask);
                break;
            case BusinessRuleTask businessRuleTask:
                HandleBusinessRuleTask(token, businessRuleTask);
                break;
            case ServiceTask serviceTask:
                HandleServiceTask(token, serviceTask);
                break;
            case ScriptTask scriptTask:
                HandleScriptTask(token, scriptTask);
                break;
            case SubProcess subProcess:
                HandleSubProcess(token, subProcess);
                break;
            case CallActivity callActivity:
                HandleCallActivity(token, callActivity);
                break;
            default:
                HandleDefaultActivity(token, activity);
                break;
        }
    }
    
    private void HandleSendTask(BPMNToken token, SendTask sendTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleReceiveTask(BPMNToken token, ReceiveTask receiveTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleUserTask(BPMNToken token, UserTask userTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleManualTask(BPMNToken token, ManualTask manualTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleBusinessRuleTask(BPMNToken token, BusinessRuleTask businessRuleTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleServiceTask(BPMNToken token, ServiceTask serviceTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleScriptTask(BPMNToken token, ScriptTask scriptTask)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleSubProcess(BPMNToken token, SubProcess subProcess)
    {
        if (token.IsWaiting) return;
        
        var startEvent = subProcess.FlowElements.OfType<StartEvent>().FirstOrDefault();
        if (startEvent != null)
        {
            _tokenManager.SetTokenWaiting(token, true);
            var t = _tokenManager.AddToken(startEvent, _objectBounds[startEvent.Id]);
            t.Parent = token;
        }
        else
        {
            base.OnTokenArrived(token);
        }
    }
    
    private void HandleCallActivity(BPMNToken token, CallActivity callActivity)
    {
        base.OnTokenArrived(token);
    }
    
    private void HandleDefaultActivity(BPMNToken token, Activity activity)
    {
        base.OnTokenArrived(token);
    }
}
*/