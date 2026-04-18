using BPMNVisualizer.Simulation.Simulators;

namespace BPMNVisualizer.Simulation;

public record RemoveTokenAction(
    BPMNToken Token
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var manager = Token.Owner ?? tokenManager;
        manager.RemoveToken(Token);
    }

    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}