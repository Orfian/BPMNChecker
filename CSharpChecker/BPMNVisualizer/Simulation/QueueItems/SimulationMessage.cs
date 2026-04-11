namespace BPMNVisualizer.Simulation;

public record SimulationMessage(
    string MessageName,
    string SourceElementId
) : ISimulationQueueItem
{
    public string DisplayName => MessageName;
    public string SourceId => SourceElementId;

    public SimulationMessage DeepClone() => this with { };
}
