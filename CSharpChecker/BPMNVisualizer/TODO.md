MUST:
- [Done] Participants (Pools and Lanes)
- Artifacts (Text Annotations, Groups)
- Label positions on Sequence Flows
- [Done] Complex Gateway
- [Done] Event Subprocess, Transaction
- [Done] Fix Expanded Subprocesses
- Collapsed SubProcesses (Second Image)
- [Done] Activity Markers
- [Done] Conditional and Default Connections
- [Done] Non-Interrupting (Dashed) Events
- [Done] Cross Pool Connections
- [Done] Export to PNG
- [In Progress] Improve Simulation UI (add autoplay, step, restart and cancel)
- [Done] Learn about multiple start events and implement accordingly
- [Done] Animation that goes on the flow lines
- [Done] Look up all Objects that can have a Default Flow and change Inclusive Gateway simulation accordingly
- [Done] Refactor to check tokens in phases (move, check, trigger) - Fix autotriggering of gateways after Inclusive and Parallel Gateways
- [Done] Change pop up window UI in simulation mode into choice arrows
- [Done] Handle timer, conditional, message, signal events in simulation mode

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
- Find out where the location of connection annotations is saved and move them accordingly