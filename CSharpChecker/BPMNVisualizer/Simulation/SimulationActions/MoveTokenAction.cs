using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record MoveTokenAction(
    BPMNToken Token,
    FlowNode TargetElement,
    SequenceFlow Flow
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;
        
        if (vars.ObjectBounds.TryGetValue(TargetElement.Id!, out var targetBounds))
        {
            var manager = Token.Owner ?? tokenManager;
            manager.MoveToken(
                Token,
                TargetElement,
                Flow,
                vars.Paths.TryGetValue(Flow.Id!, out var path) ? path : null,
                targetBounds
            );
                
            actionList.CheckBoundaryEvents(Token);
        }
    }
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}