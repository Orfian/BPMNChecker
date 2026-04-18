using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record SendMessageAction(
    BPMNToken Token,
    string MessageName
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;

        var message = new SimulationMessage(
            MessageName,
            Token.CurrentElement?.Id ?? "Unknown"
        );
        actionList.MessageQueue.Enqueue(message);
        vars.Logger.Information("Message {MessageName} sent from {SourceId}", message.MessageName, message.SourceElementId);
    }
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}