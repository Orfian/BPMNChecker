using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record SendSignalAction(
    BPMNToken Token,
    string SignalName
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;

        var signal = new SimulationSignal(
            SignalName,
            Token.CurrentElement?.Id ?? "Unknown"
        );
        actionList.SignalQueue.Enqueue(signal);
        vars.Logger.Information("Signal {SignalName} sent from {SourceId}", signal.SignalName, signal.SourceElementId);
    }
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}