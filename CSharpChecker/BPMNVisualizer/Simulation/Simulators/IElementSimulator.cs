namespace BPMNVisualizer.Simulation.Simulators;

public interface IElementSimulator
{
    //void OnTokenArrived(BPMNToken token);
    void Evaluate(BPMNToken token, IList<SimulationAction> actions);
}