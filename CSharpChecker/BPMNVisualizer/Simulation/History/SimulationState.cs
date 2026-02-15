namespace BPMNVisualizer.Simulation.History;

public class SimulationState
{
    public int StepIndex { get; set; }

    public List<BPMNToken> Tokens { get; set; } = new();

    public List<GatewayChoice> PendingGatewayChoices { get; set; } = new();

    public Queue<SimulationMessage> MessageQueue { get; set; } = new();
    public Queue<SimulationSignal> SignalQueue { get; set; } = new();
    
    public List<string> TriggeredCodeElements { get; set; } = new();
}
