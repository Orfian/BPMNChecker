namespace BPMNVisualizer.Simulation;

public record SimulationMessage(
    string MessageName,
    string SourceElementId,
    string? TargetElementId = null
) : ISimulationQueueItem
{
    public string DisplayName => MessageName;
    public string SourceId => SourceElementId;
}
