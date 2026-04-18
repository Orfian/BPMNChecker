using System.Windows;
using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record SpawnCollapsedSubProcessAction( //for subprocesses, call activities, etc.
    SubProcess SubProcess,
    BPMNToken ParentToken
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;
        
        var subDiagram = vars.Model.Definition?.Diagrams
            .FirstOrDefault(d => d.Plane?.BpmnElement?.Id == SubProcess.Id);

        if (subDiagram == null) return;

        // Reuse existing window or create a new one
        if (!vars.SubProcessWindows.TryGetValue(SubProcess.Id, out var window))
        {
            window = new SubProcessWindow
            {
                Owner = Application.Current.MainWindow,
                Title = $"Sub-Process: {SubProcess.Name ?? SubProcess.Id}"
            };
            window.RenderSubProcess(subDiagram);
            vars.SubProcessWindows[SubProcess.Id] = window;
        }

        window.StartSimulation(SubProcess, ParentToken);

        if (!window.IsVisible)
            window.Show();
        else
            window.Activate();
    }

    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken.DeepClone() };
}