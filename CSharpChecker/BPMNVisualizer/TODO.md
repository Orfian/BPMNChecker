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
- [In Process] Improve Simulation UI (add autoplay, step, restart and cancel)
- [In Process] Learn about multiple start events and implement accordingly
- [Done] Animation that goes on the flow lines
- [Done] Look up all Objects that can have a Default Flow and change Inclusive Gateway simulation accordingly
- Fix autotriggering of gateways after Inclusive and Parallel Gateways

WORKINGS:
- [Done] Parallel Gateways join incoming tokens and wait for all the routes
- [Done] Inclusive Gateways join incoming tokens and wait for all the possible routes (all that can arrive based on previous decisions)
- [In Progress] Learn about and implement ComplexGateways (including joining behavior)

NICE TO HAVE:
- Zoom In/Out
- Pan
- Minimap(?)
- [In Progress] Add text like "(Event)" or "(Gateway)" and "Default" next to choices in simulation mode
- Dark mode