using BPMNModel.Model;

namespace BPMNVisualizer.Simulation;

public abstract record SimulationAction
{
    public abstract SimulationAction DeepClone();
}

public record MoveTokenAction(
    BPMNToken Token,
    FlowNode TargetElement,
    SequenceFlow Flow
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record SplitTokenAction( //for parallel gateways, multiple flows, etc.
    FlowNode SourceElement,
    FlowNode TargetElement,
    SequenceFlow Flow,
    BPMNToken? ParentToken = null
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken?.DeepClone() };
}

public record SpawnTokenAction( //for subprocesses, call activities, etc.
    FlowNode TargetElement,
    BPMNToken ParentToken
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken.DeepClone() };
}

public record SpawnCollapsedSubProcessAction( //for subprocesses, call activities, etc.
    SubProcess SubProcess,
    BPMNToken ParentToken
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken.DeepClone() };
}

public record RemoveTokenAction(
    BPMNToken Token
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record SetTokenWaitingAction(
    BPMNToken Token,
    bool IsWaiting
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record RequestGatewayChoiceAction(
    BPMNToken Token,
    Gateway Gateway,
    IEnumerable<SequenceFlow> OutgoingFlows,
    bool MultiSelect,
    SequenceFlow? DefaultFlow
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record ResolveGatewayChoiceAction(
    GatewayChoice GatewayChoice
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { GatewayChoice = GatewayChoice.DeepClone() };
}

public record DelayTokenAction(
    BPMNToken Token,
    FlowNode Element
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record SendMessageAction(
    BPMNToken Token,
    string MessageName
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record SendSignalAction(
    BPMNToken Token,
    string SignalName
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}
