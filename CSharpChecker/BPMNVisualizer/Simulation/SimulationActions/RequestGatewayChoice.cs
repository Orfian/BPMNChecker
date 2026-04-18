using BPMNModel.Model;
using BPMNVisualizer.Simulation.Simulators;

namespace BPMNVisualizer.Simulation;

public record RequestGatewayChoiceAction(
    BPMNToken Token,
    Gateway Gateway,
    IEnumerable<SequenceFlow> OutgoingFlows,
    bool MultiSelect,
    SequenceFlow? DefaultFlow
) : SimulationAction
{
    public override void Commit(SimulationActionList actionList, TokenManager tokenManager, ActivitySimulator activitySimulator, EventSimulator eventSimulator, GatewaySimulator gatewaySimulator)
    {
        var manager = Token.Owner ?? tokenManager;
            
        var gatewayChoice = new GatewayChoice
        {
            Token = Token,
            Gateway = Gateway,
            OutgoingFlows = OutgoingFlows,
            MultiSelect = MultiSelect,
            DefaultFlow = DefaultFlow
        };

        manager.ShowGatewayChoiceIndicators(gatewayChoice);
                    
        if (gatewayChoice.DefaultFlow != null)
        {
            var defaultIndicator = gatewayChoice.Indicators
                .FirstOrDefault(ind => ind.Flow == gatewayChoice.DefaultFlow);
            if (defaultIndicator != null && defaultIndicator.Visual != null)
            {
                defaultIndicator.Selected = true;
                manager.SetIndicatorColor(defaultIndicator.Visual, true);
            }
        }
        else
        {
            if (gatewayChoice.Gateway is not ComplexGateway && gatewayChoice.Gateway is not EventBasedGateway)
            {
                var ind = gatewayChoice.Indicators.FirstOrDefault();
                if (ind != null && ind.Visual != null)
                {
                    ind.Selected = true;
                    manager.SetIndicatorColor(ind.Visual, true);
                }
            }
        }
                    
        actionList.PendingGatewayChoices.Add(gatewayChoice);

        actionList.ResolveGatewayChoiceActions.Add(new ResolveGatewayChoiceAction(gatewayChoice));
    }
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}