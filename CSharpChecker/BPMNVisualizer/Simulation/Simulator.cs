using System.Windows;
using System.Collections.ObjectModel;
using BPMNModel.Model;
using BPMNVisualizer.Simulation.History;
using BPMNVisualizer.Simulation.Simulators;
using BPMNVisualizer.Utility;

namespace BPMNVisualizer.Simulation;

public class Simulator
{
    private readonly SharedVariables _vars;
    
    private readonly TokenManager _tokenManager;

    private readonly ActivitySimulator _activitySimulator;
    private readonly EventSimulator _eventSimulator;
    private readonly GatewaySimulator _gatewaySimulator;

    private readonly SimulationActionList _actions;

    public readonly ObservableCollection<SimulationState> History = new();
    private int _currentStepIndex = -1;

    public Simulator(TokenManager tokenManager)
    {
        _vars = SharedVariables.Instance;
        _tokenManager = tokenManager;
        _actions = new SimulationActionList(_tokenManager,null, null, null);
        
        _activitySimulator = new ActivitySimulator(_tokenManager, _actions);
        _eventSimulator = new EventSimulator(_tokenManager, _actions);
        _gatewaySimulator = new GatewaySimulator(_tokenManager, _actions);
        
        _actions.SetSimulators(_activitySimulator, _eventSimulator, _gatewaySimulator);
    }
    
    public void FirstStep()
    {
        var allStartEvents = _vars.Model.AllObjectsWithIds.Values
            .OfType<StartEvent>()
            .ToList();

        var subProcessStartEventIds = _vars.Model.AllObjectsWithIds.Values
            .OfType<SubProcess>()
            .SelectMany(sp => sp.FlowElements.OfType<StartEvent>())
            .Select(se => se.Id)
            .ToHashSet();
        
        var userStartEvents = allStartEvents
            .Where(Helpers.IsUserStart)
            .Where(se => !subProcessStartEventIds.Contains(se.Id))
            .ToList();

        var startEvents = allStartEvents
            .Where(se => !subProcessStartEventIds.Contains(se.Id))
            .Where(se => !Helpers.IsUserStart(se))
            .ToList();

        if (!startEvents.Any())
        {
            _vars.Logger.Warning("No start events found in BPMN model.");
            return;
        }

        foreach (var startEvent in startEvents)
        {
            if (_vars.ObjectBounds.TryGetValue(startEvent.Id!, out var bounds))
            {
                Console.WriteLine("Bounds: " + bounds + ", Id: " + _tokenManager.AddToken(startEvent, bounds).Id);
            }
        }
        
        if (userStartEvents.Any())
        {
            foreach (var startEvent in userStartEvents)
            {
                _eventSimulator.SpawnStartEventIndicator(startEvent, null);
            }
        }
        
        SaveState(_tokenManager.GetAllTokens());
    }
    
    public void NextStep()
    {
        var tokens = _tokenManager.GetAllTokens().ToList();
        if (!tokens.Any())
        {
            MessageBox.Show("No more tokens.", "Next Step not allowed", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (_currentStepIndex >= 0 && _currentStepIndex < History.Count)
        {
            var oldState = History[_currentStepIndex];
            oldState.PendingGatewayChoices = _actions.PendingGatewayChoices.Select(c => c.DeepClone()).ToList();
        }
        
        foreach (var token in tokens)
        {
            token.IsEvaluated = false;
        }

        foreach (var token in tokens)
        {
            if (token.IsEvaluated || token.CurrentElement == null)
                continue;
            
            var simulator = GetElementSimulator(token.CurrentElement);
            simulator.Evaluate(token);
            token.IsEvaluated = true;
        }

        _actions.CommitActions();
        
        SaveState(_tokenManager.GetAllTokens());
    }
    
    
    private IElementSimulator GetElementSimulator(BaseElement element)
    {
        return element switch
        {
            Activity => _activitySimulator,
            Event => _eventSimulator,
            Gateway => _gatewaySimulator,
            _ => throw new NotSupportedException(
                $"No simulator available for element type: {element.GetType().Name}")
        };
    }
    
    public void ClearAllActions()
    {
        _actions.ClearActions();
    }
    
    public void ClearPendingChoices()
    {
        _actions.ClearPendingGatewayChoices();
    }

    public void SaveState(IEnumerable<BPMNToken> tokens)
    {
        // If we are saving state from a point in history (not the end), truncate future history
        if (_currentStepIndex != -1 && _currentStepIndex < History.Count - 1)
        {
            while (History.Count > _currentStepIndex + 1)
            {
                History.RemoveAt(History.Count - 1);
            }
        }

        var state = new SimulationState
        {
            StepIndex = History.Count,
            Tokens = tokens.Select(t => t.DeepClone()).ToList(),
            PendingGatewayChoices = _actions.PendingGatewayChoices.Select(c => c.DeepClone()).ToList(),
            MessageQueue = new Queue<SimulationMessage>(_actions.MessageQueue.Select(m => m.DeepClone())),
            SignalQueue = new Queue<SimulationSignal>(_actions.SignalQueue.Select(s => s.DeepClone())),
            TriggeredCodeElements = GetTriggeredCodeElements(tokens)
        };
        
        History.Add(state);
        _currentStepIndex = History.Count - 1;
    }
    
    private List<string> GetTriggeredCodeElements(IEnumerable<BPMNToken> tokens)
    {
        var elements = new HashSet<string>();

        // 1. Check Sequence Flows (Edges) that were traversed in this step
        // We look at MoveTokenAction and SplitTokenAction in _actions

        foreach (var action in _actions.MoveTokenActions)
        {
            var flow = action.Flow;
            if (flow != null && flow.ConditionExpression is FormalExpression expr && !string.IsNullOrWhiteSpace(expr.Body?.Value))
            {
                elements.Add($"Flow: {flow.Id} (Condition)");
            }
        }
        
        foreach (var action in _actions.SplitTokenActions)
        {
            var flow = action.Flow;
            if (flow != null && flow.ConditionExpression is FormalExpression expr && !string.IsNullOrWhiteSpace(expr.Body?.Value))
            {
                elements.Add($"Flow: {flow.Id} (Condition)");
            }
        }

        // 2. Check Elements (Nodes) where tokens are currently located IF they were evaluated/active this step
        // Note: tokens passed to SaveState are the current state tokens.
        foreach (var token in tokens)
        {
            // If token is at an element, check if that element has code
            if (token.CurrentElement is BaseElement el && Helpers.HasScript(el))
            {
                if (el is ScriptTask st)
                    elements.Add($"Script Task: {st.Name ?? st.Id}");
                else if (el is ServiceTask srv)
                    elements.Add($"Service Task: {srv.Name ?? srv.Id} (Expression)");
                else
                    elements.Add($"Element: {el.Id} (Listeners)");
            }
        }

        return elements.OrderBy(x => x).ToList();
    }
    
    public void ClearHistory()
    {
        History.Clear();
        _currentStepIndex = -1;
    }

    public void LoadState(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= History.Count)
            return;
            
        var state = History[stepIndex];
        _currentStepIndex = stepIndex;

        // 1. Clear current state
        _tokenManager.ClearAllTokens();
        ClearPendingChoices();
        _actions.ClearAll();
        
        // 2. Restore Queues
        foreach(var m in state.MessageQueue) _actions.MessageQueue.Enqueue(m.DeepClone());
        foreach(var s in state.SignalQueue) _actions.SignalQueue.Enqueue(s.DeepClone());
        
        // 3. Build lookup: element ID -> collapsed SubProcess ID
        var elementToCollapsedSubProcess = new Dictionary<string, SubProcess>();
        var collapsedSubProcesses = _vars.Model.AllObjectsWithIds.Values
            .OfType<SubProcess>()
            .Where(sp => _vars.Shapes.TryGetValue(sp.Id, out var shape) && shape.IsExpanded == false)
            .ToList();

        foreach (var sp in collapsedSubProcesses)
        {
            foreach (var fe in sp.FlowElements)
            {
                if (fe.Id != null)
                    elementToCollapsedSubProcess[fe.Id] = sp;
            }
        }
        
        // 4. Restore Tokens
        var oldToNew = new Dictionary<string, BPMNToken>();
        
        foreach (var historyToken in state.Tokens)
        {
             if (historyToken.CurrentElement != null && historyToken.Id != null && _vars.ObjectBounds.TryGetValue(historyToken.CurrentElement.Id!, out var bounds))
             {
                 //_vars.ObjectBounds.TryGetValue(startEvent.Id!, out var bounds)
                 Console.WriteLine("Bounds: " + bounds + ", Id: " + historyToken.Id);
                 var manager = _tokenManager;
                 if (historyToken.CurrentElement.Id != null && elementToCollapsedSubProcess.TryGetValue(historyToken.CurrentElement.Id, out var owningSubProcess))
                 {
                     manager = GetOrCreateSubProcessWindow(owningSubProcess).TokenManager;
                 }

                 var newToken = manager.AddToken(historyToken.CurrentElement, bounds);
                 newToken.Id = historyToken.Id;
                 newToken.IsWaiting = historyToken.IsWaiting;
                 newToken.IsEvaluated = historyToken.IsEvaluated;
                 newToken.CurrentSequenceFlow = historyToken.CurrentSequenceFlow;

                  if (historyToken.CurrentSequenceFlow?.Id != null && _vars.Paths.TryGetValue(historyToken.CurrentSequenceFlow.Id, out var path) && path != null) {
                     var points = path.ToList();
                     if (points.Any()) {
                         var lastPoint = points.Last();
                         newToken.Owner?.SetTokenPosition(newToken, lastPoint);
                     }
                 }

                 (newToken.Owner ?? _tokenManager).SetTokenWaiting(newToken, newToken.IsWaiting);

                 oldToNew[historyToken.Id] = newToken;
             }
        }
        
        // Fix parents
        foreach (var historyToken in state.Tokens)
        {
            if (historyToken.Parent != null && historyToken.Parent.Id != null && oldToNew.TryGetValue(historyToken.Parent.Id, out var newParent))
            {
                // We need to look up the NEW token corresponding to historyToken
                if (historyToken.Id != null && oldToNew.TryGetValue(historyToken.Id, out var newToken))
                {
                    newToken.Parent = newParent;
                }
            }
        }

        // 5. Restore Pending Gateway Choices
        var newPendingChoices = state.PendingGatewayChoices.Select(c => c.DeepClone()).ToList();
        
        foreach (var choice in newPendingChoices)
        {
             // Fix token reference
             if (choice.Token?.Id != null && oldToNew.TryGetValue(choice.Token.Id, out var newToken))
             {
                 choice.Token = newToken;
             }

             var preservedSelections = choice.Indicators.Where(i => i.Selected && i.Flow?.Id != null).Select(i => i.Flow!.Id).ToHashSet();
             choice.Indicators.Clear();
             
             var choiceManager = choice.Token?.Owner ?? _tokenManager;
             
             // Recreate indicators (visuals + objects)
             choiceManager.ShowGatewayChoiceIndicators(choice);
             
             // Restore selection
             foreach (var ind in choice.Indicators)
             {
                 if (ind.Flow?.Id != null && preservedSelections.Contains(ind.Flow.Id) && ind.Visual != null)
                 {
                     ind.Selected = true;
                     choiceManager.SetIndicatorColor(ind.Visual, true);
                 }
             }
             
             _actions.PendingGatewayChoices.Add(choice);
             
             bool actionFound = false;
             /*
             for (int i = 0; i < _priorityActions.Count; i++)
             {
                 if (_priorityActions[i] is ResolveGatewayChoiceAction action)
                 {
                     if (action.GatewayChoice.Token?.Id != null && action.GatewayChoice.Gateway?.Id != null && choice.Token?.Id != null && choice.Gateway?.Id != null &&
                         action.GatewayChoice.Token.Id == choice.Token.Id && action.GatewayChoice.Gateway.Id == choice.Gateway.Id)
                     {
                         _priorityActions[i] = action with { GatewayChoice = choice };
                         actionFound = true;
                     }
                 }
             }
             */
             
             for (int i = 0; i < _actions.PendingGatewayChoices.Count; i++)
             {
                 if (_actions.PendingGatewayChoices[i].Token?.Id != null && _actions.PendingGatewayChoices[i].Gateway?.Id != null && choice.Token?.Id != null && choice.Gateway?.Id != null &&
                     _actions.PendingGatewayChoices[i].Token.Id == choice.Token.Id && _actions.PendingGatewayChoices[i].Gateway.Id == choice.Gateway.Id)
                 {
                     _actions.PendingGatewayChoices[i] = choice;
                     actionFound = true;
                 }
             }
             
             if (!actionFound)
             {
                _actions.PendingGatewayChoices.Add(choice);
             }
        }
    }
    
    /// <summary>
    /// Gets an existing SubProcessWindow or creates and registers a new one for the given subprocess.
    /// Ensures the window is rendered and visible.
    /// </summary>
    private SubProcessWindow GetOrCreateSubProcessWindow(SubProcess subProcess)
    {
        var vars = SharedVariables.Instance;
        
        if (vars.SubProcessWindows.TryGetValue(subProcess.Id, out var window))
        {
            // Ensure it has a TokenManager initialized
            if (window.TokenManager == null)
                window.TokenManager = new TokenManager(window.SubProcessCanvas);
            
            if (!window.IsVisible)
                window.Show();
                
            return window;
        }

        var subDiagram = vars.Model.Definition?.Diagrams
            .FirstOrDefault(d => d.Plane?.BpmnElement?.Id == subProcess.Id);

        window = new SubProcessWindow
        {
            Owner = Application.Current.MainWindow,
            Title = $"Sub-Process: {subProcess.Name ?? subProcess.Id}"
        };

        if (subDiagram != null)
            window.RenderSubProcess(subDiagram);

        window.TokenManager = new TokenManager(window.SubProcessCanvas);
        vars.SubProcessWindows[subProcess.Id] = window;
        window.Show();

        return window;
    }
}
