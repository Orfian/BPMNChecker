using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public record ResolveGatewayChoiceAction(
    GatewayChoice GatewayChoice
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var vars = SharedVariables.Instance;
        
        var choice = GatewayChoice;
        var manager = choice.Token?.Owner ?? tokenManager;
            
        var selectedFlows = choice.Indicators
            .Where(ind => ind.Selected && ind.Flow != null)
            .Select(ind => ind.Flow!)
            .ToList();
                
        switch (choice.Gateway)
        {
            case ParallelGateway pg:
                vars.Logger.Warning("Unexpected gateway action: {ActionType}", choice.Gateway.GetType().Name);
                break;
            case ExclusiveGateway eg:
                gatewaySimulator.ResolveExclusiveGateway(choice.Token, eg, selectedFlows);
                break;
            case InclusiveGateway ig:
                gatewaySimulator.ResolveInclusiveGateway(choice.Token, ig, selectedFlows);
                break;
            case ComplexGateway cg:
                gatewaySimulator.ResolveComplexGateway(choice.Token, cg, selectedFlows);
                break;
            case EventBasedGateway ebg:
                gatewaySimulator.ResolveEventBasedGateway(choice.Token, ebg, selectedFlows);
                break;
            default:
                vars.Logger.Warning("Unknown gateway action: {ActionType}", choice.Gateway.GetType().Name);
                break;
        }

        foreach (var indicator in GatewayChoice.Indicators)
        {
            if (indicator.Visual != null)
            {
                manager.RemoveArrowIndicator(indicator.Visual);
            }
        }
            
        actionList.PendingGatewayChoices.Remove(GatewayChoice);
            
        var requestAction = actionList.RequestGatewayChoiceActions.FirstOrDefault(ra => ra.Token == choice.Token);
        if (requestAction != null)
            actionList.RequestGatewayChoiceActions.Remove(requestAction);
    }
    public override SimulationAction DeepClone() => this with { GatewayChoice = GatewayChoice.DeepClone() };
}