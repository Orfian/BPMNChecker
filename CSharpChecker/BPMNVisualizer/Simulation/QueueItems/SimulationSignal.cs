namespace BPMNVisualizer.Simulation;

public record SimulationSignal(
    string SignalName,
    string SourceElementId
) : ISimulationQueueItem
{
    public string DisplayName => SignalName;
    public string SourceId => SourceElementId;

    public SimulationSignal DeepClone() => this with { };
}
