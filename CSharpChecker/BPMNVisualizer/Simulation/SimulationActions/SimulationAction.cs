using BPMNVisualizer.Simulation.Simulators;

namespace BPMNVisualizer.Simulation;

public abstract record SimulationAction
{
    public abstract void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator);
    public abstract SimulationAction DeepClone();
}