using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;

namespace BPMNVisualizer.Simulation;

public record DelayTokenAction(
    BPMNToken Token,
    FlowNode Element
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var manager = Token.Owner ?? tokenManager;
            
        var trigger = new ElementTrigger
        {
            Token = Token,
            Element = Element
        };
            
        manager.ShowEventTriggerIndicator(trigger);
            
        var arrow = trigger.Indicator.Visual;
        if (arrow == null) return;
            
        arrow.MouseDown += (s, e) =>
        {
            actionList.ResolveElementTrigger(trigger);
        };
            
        actionList.PendingElementTriggers.Add(trigger);
    }
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}