MUST:
- [Done] Participants (Pools and Lanes)
- Artifacts (Text Annotations, Groups)
- [Done] Label positions on Sequence Flows
- [Done] Label positions on elements
- [Done] Complex Gateway
- [Done] Event Subprocess, Transaction
- [Done] Fix Expanded Subprocesses
- [Done] Collapsed SubProcesses (Second Image)
- [Done] Activity Markers
- [Done] Conditional and Default Connections
- [Done] Non-Interrupting (Dashed) Events
- [Done] Cross Pool Connections
- [Done] Export to PNG
- [Done] Improve Simulation UI (add step, restart and cancel)
- [Done] Learn about multiple start events and implement accordingly
- [Done] Animation that goes on the flow lines
- [Done] Look up all Objects that can have a Default Flow and change Inclusive Gateway simulation accordingly
- [Done] Refactor to check tokens in phases (move, check, trigger) - Fix autotriggering of gateways after Inclusive and Parallel Gateways
- [Done] Change pop up window UI in simulation mode into choice arrows
- [Done] Handle timer, conditional, message, signal events in simulation mode
- [Done] Handle Message, Signal, Conditional, Timer Start Events in simulation mode
- [Done] Add the same message and signal queue for start events as for intermediate events
- [Done] Don't remove indicators for Message, Signal, Conditional, Timer Start Events when they are triggered in simulation mode
- Somehow indicate multiple tokens on the same element in simulation mode (e.g. by stacking or numbering tokens)
- Fix the issue with Gateways
- [Done] Add message, signal, conditional and timer start indicators into history
- Handle Boundary Events
- Code refactoring and cleanup

WORKINGS:
- [Done] Parallel Gateways join incoming tokens and wait for all the routes
- [Done] Inclusive Gateways join incoming tokens and wait for all the possible routes (all that can arrive based on previous decisions)
- [Done] Learn about and implement ComplexGateways (including joining behavior)

NICE TO HAVE:
- Zoom In/Out
- Pan
- Minimap(?)
- [Deprecated] Add text like "(Event)" or "(Gateway)" and "Default" next to choices in simulation mode
- Dark mode
