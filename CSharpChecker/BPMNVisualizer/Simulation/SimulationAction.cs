using BPMNModel.Model;

namespace BPMNVisualizer.Simulation;

public abstract record SimulationAction;

public record MoveTokenAction(
    BPMNToken Token,
    BaseElement TargetElement,
    SequenceFlow Flow
) : SimulationAction;

public record SplitTokenAction( //for parallel gateways, multiple flows, etc.
    BaseElement SourceElement,
    BaseElement TargetElement,
    SequenceFlow Flow,
    BPMNToken? ParentToken = null
) : SimulationAction;

public record SpawnTokenAction( //for subprocesses, call activities, etc.
    BaseElement TargetElement,
    BPMNToken ParentToken
) : SimulationAction;

public record RemoveTokenAction(
    BPMNToken Token
) : SimulationAction;

public record SetTokenWaitingAction(
    BPMNToken Token,
    bool IsWaiting
) : SimulationAction;

