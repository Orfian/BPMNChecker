using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record SplitTokenAction( //for parallel gateways, multiple flows, etc.
    FlowNode SourceElement,
    FlowNode TargetElement,
    SequenceFlow Flow,
    BPMNToken? ParentToken = null
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;
        
        if (vars.ObjectBounds.TryGetValue(SourceElement.Id!, out var sourceBounds))
        {
            var manager = ParentToken?.Owner ?? tokenManager;
            var newToken = manager.AddToken(SourceElement, sourceBounds);
            newToken.Parent = ParentToken;
                        
            if (vars.ObjectBounds.TryGetValue(TargetElement.Id!, out var targetBounds))
            {
                manager.MoveToken(
                    newToken,
                    TargetElement,
                    Flow,
                    vars.Paths.TryGetValue(Flow.Id!, out var path) ? path : null,
                    targetBounds
                );
                    
                actionList.CheckBoundaryEvents(newToken);
            }
        }
    }
    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken?.DeepClone() };
}