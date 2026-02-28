using BPMNModel.Model;

namespace BPMNVisualizer.Simulation;

public abstract record SimulationAction
{
    public abstract SimulationAction DeepClone();
}

public record MoveTokenAction(
    BPMNToken Token,
    BaseElement TargetElement,
    SequenceFlow Flow
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { Token = Token.DeepClone() };
}

public record SplitTokenAction( //for parallel gateways, multiple flows, etc.
    BaseElement SourceElement,
    BaseElement TargetElement,
    SequenceFlow Flow,
    BPMNToken? ParentToken = null
) : SimulationAction
{
    public override SimulationAction DeepClone() => this with { ParentToken = ParentToken?.DeepClone() };
}

public record SpawnTokenAction( //for subprocesses, call activities, etc.
    BaseElement TargetElement,
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

public record EventDelayAction(
    BPMNToken Token,
    Event Event
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
