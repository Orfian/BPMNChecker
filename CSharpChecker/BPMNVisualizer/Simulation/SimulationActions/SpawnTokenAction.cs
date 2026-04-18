using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record SpawnTokenAction( //for subprocesses, call activities, etc.
    FlowNode TargetElement,
    BPMNToken ParentToken
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;

        if (vars.ObjectBounds.TryGetValue(TargetElement.Id!, out var targetBounds))
        {
            var manager = ParentToken?.Owner ?? tokenManager;
            var newToken = manager.AddToken(TargetElement, targetBounds);
            newToken.Parent = ParentToken;
        }
    }
    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken.DeepClone() };
}