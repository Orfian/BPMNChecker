using BPMNVisualizer.Simulation.Simulators;

namespace BPMNVisualizer.Simulation;

public record SetTokenWaitingAction(
    BPMNToken Token,
    bool IsWaiting
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var manager = Token.Owner ?? tokenManager;
        manager.SetTokenWaiting(Token, IsWaiting);
    }
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}