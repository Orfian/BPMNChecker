namespace BPMNVisualizer.Simulation;

public interface ISimulationQueueItem
{
    string DisplayName { get; }
    string SourceId { get; }
}

