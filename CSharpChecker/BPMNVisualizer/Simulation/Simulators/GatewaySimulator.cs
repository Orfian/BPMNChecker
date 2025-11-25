using System.Windows;
using BPMNModel;
using BPMNModel.Model;
using Serilog;
using Point = System.Windows.Point;


namespace BPMNVisualizer.Simulation.Simulators;

public class GatewaySimulator : DefaultSimulator
{
    private readonly Dictionary<string, Dictionary<string, Queue<BPMNToken>>> _joinBuffers = new();

    public GatewaySimulator(ILogger logger, TokenManager tokenManager, Dictionary<string, Rect> objectBounds, Dictionary<string, IEnumerable<Point>> paths)
        : base(logger, tokenManager, objectBounds, paths)
    {}
    
    public override void OnTokenArrived(BPMNToken token)
    {
        if (token == null || token.CurrentElement == null)
        {
            _logger.Warning("Token or its current element is null in GatewaySimulator.");
            return;
        }
        
        if (token.CurrentElement is not Gateway gateway)
        {
            base.OnTokenArrived(token);
            return;
        }
        
        var outgoingFlows = _tokenManager.GetOutgoingFlows(token.CurrentElement);
        if (!outgoingFlows.Any())
        {
            _logger.Information($"Gateway {gateway.Id} has no outgoing flows.");
            return;
        }
        
        switch (gateway)
        {
            case ParallelGateway parallelGateway:
                HandleParallelGateway(token, parallelGateway, outgoingFlows);
                break;
            case ExclusiveGateway exclusiveGateway:
                HandleExclusiveGateway(token, exclusiveGateway, outgoingFlows);
                break;
            case InclusiveGateway inclusiveGateway:
                HandleInclusiveGateway(token, inclusiveGateway, outgoingFlows);
                break;
            case ComplexGateway complexGateway:
                HandleComplexGateway(token, complexGateway, outgoingFlows);
                break;
            case EventBasedGateway eventBasedGateway:
                HandleEventBasedGateway(token, eventBasedGateway, outgoingFlows);
                break;
            default:
                _logger.Warning($"Unsupported gateway type: {gateway.GetType().Name}");
                base.OnTokenArrived(token);
                break;
        }
    }

    private void HandleParallelGateway(BPMNToken token, ParallelGateway gateway, IEnumerable<SequenceFlow> outgoingFlows)
    {
        var incomingFlows = _tokenManager.GetIncomingFlows(gateway);
        bool joining = incomingFlows.Count() > 1;
        
        if (!joining)
        {
            base.OnTokenArrived(token);
            return;
        }

        if (!_joinBuffers.TryGetValue(gateway.Id, out var buffer))
        {
            buffer = new Dictionary<string, Queue<BPMNToken>>();
            _joinBuffers[gateway.Id] = buffer;
        }
        
        foreach (var flow in incomingFlows)
        {
            if (!buffer.ContainsKey(flow.Id))
            {
                buffer[flow.Id] = new Queue<BPMNToken>();
            }
        }
        
        var incomingFlowId = token.CurrentSequenceFlow?.Id;
        if (!string.IsNullOrEmpty(incomingFlowId) && buffer.ContainsKey(incomingFlowId))
        {
            buffer[incomingFlowId].Enqueue(token);
            _tokenManager.SetTokenWaiting(token, true);
        }
        
        bool allTokensArrived = incomingFlows.All(flow => buffer.ContainsKey(flow.Id) && buffer[flow.Id].Count > 0);

        if (allTokensArrived)
        {
            var tokensToJoin = new List<BPMNToken>();
            foreach (var flow in incomingFlows)
            {
                tokensToJoin.Add(buffer[flow.Id].Dequeue());
            }
            
            bool first = true;
            
            foreach (var tok in tokensToJoin)
            {
                if (first)
                {
                    base.OnTokenArrived(tok);
                    first = false;
                    continue;
                }
                
                _tokenManager.RemoveToken(tok);
            }
            
            var emptyKeys = buffer.Where(kv => kv.Value.Count == 0).Select(kv => kv.Key).ToList();
            foreach (var key in emptyKeys) buffer.Remove(key);
            if (buffer.Count == 0) _joinBuffers.Remove(gateway.Id);
        }
    }

    private void HandleExclusiveGateway(BPMNToken token, ExclusiveGateway gateway, IEnumerable<SequenceFlow> outgoingFlows)
    {
        if (outgoingFlows.Count() == 1)
        {
            base.OnTokenArrived(token);
            return;
        }
        
        var choices = outgoingFlows
            .Select(f => f.Name ?? f.Id)
            .ToList();

        if (choices.Count == 0) return;

        var choice = ShowChoiceDialog("Exclusive Gateway", "Choose one outgoing flow:", choices);
        if (choice == null) return;

        var chosenFlow = outgoingFlows.FirstOrDefault(f => (f.Name ?? f.Id) == choice);
        if (chosenFlow == null) return;

        var next = _tokenManager.GetTargetElement(chosenFlow);
        if (next == null) return;

        if (_objectBounds.TryGetValue(next.Id, out var bounds))
        {
            var path = _paths.ContainsKey(chosenFlow.Id) ? _paths[chosenFlow.Id] : null;
            _tokenManager.MoveToken(token, next, chosenFlow, path, bounds);
        }
    }
    
    private void HandleInclusiveGateway(BPMNToken token, InclusiveGateway gateway, IEnumerable<SequenceFlow> outgoingFlows)
    {
        var incomingFlows = _tokenManager.GetIncomingFlows(gateway);
        bool joining = incomingFlows.Count() > 1;
        
        if (!joining)
        {
            SplitInclusiveGateway(token, gateway, outgoingFlows);
            return;
        }
        
        if (!_joinBuffers.TryGetValue(gateway.Id, out var buffer))
        {
            buffer = new Dictionary<string, Queue<BPMNToken>>();
            _joinBuffers[gateway.Id] = buffer;
        }
        
        foreach (var flow in incomingFlows)
        {
            if (!buffer.ContainsKey(flow.Id))
            {
                buffer[flow.Id] = new Queue<BPMNToken>();
            }
        }
        
        var incomingFlowId = token.CurrentSequenceFlow?.Id;
        if (!string.IsNullOrEmpty(incomingFlowId) && buffer.ContainsKey(incomingFlowId))
        {
            buffer[incomingFlowId].Enqueue(token);
            _tokenManager.SetTokenWaiting(token, true);
        }
        
        bool allTokensArrived = incomingFlows.All(flow => buffer.ContainsKey(flow.Id) && buffer[flow.Id].Count > 0);
        if (!allTokensArrived)
        {
            // check if any tokens are able to traverse here
            // check all tokens in the system (skipping those at this gateway)
            // if any token is at an element that can reach this gateway, we wait
            var allTokens = _tokenManager.GetAllTokens();
            if (allTokens.Count == 1)
            {
                allTokensArrived = true;
            }
            else
            {
                allTokensArrived = true;
                
                foreach (var tok in allTokens)
                {
                    if (tok.CurrentElement.Id == gateway.Id && tok.IsWaiting) continue;

                    if(_tokenManager.IsReachable(tok.CurrentElement, gateway))
                    {
                        allTokensArrived = false;
                        break;
                    }
                }
            }
        }
        
        if (allTokensArrived)
        {
            var tokensToJoin = new List<BPMNToken>();
            foreach (var flow in incomingFlows)
            {
                if (buffer.TryGetValue(flow.Id, out var q) && q.Count > 0)
                {
                    tokensToJoin.Add(q.Dequeue());
                }
            }
            
            if (tokensToJoin.Count == 0) return;
            
            bool first = true;
            
            foreach (var tok in tokensToJoin)
            {
                if (first)
                {
                    SplitInclusiveGateway(tok, gateway, outgoingFlows);
                    first = false;
                    continue;
                }
                
                _tokenManager.RemoveToken(tok);
            }
            
            var emptyKeys = buffer.Where(kv => kv.Value.Count == 0).Select(kv => kv.Key).ToList();
            foreach (var key in emptyKeys) buffer.Remove(key);
            if (buffer.Count == 0) _joinBuffers.Remove(gateway.Id);
        }
    }
    
    private void SplitInclusiveGateway(BPMNToken token, InclusiveGateway gateway, IEnumerable<SequenceFlow> outgoingFlows)
    {
        if (outgoingFlows.Count() == 1)
        {
            base.OnTokenArrived(token);
            return;
        }
        
        var choices = outgoingFlows
            .Select(f => f.Name ?? f.Id)
            .ToList();

        if (choices.Count == 0) return;

        var defaultFlow = outgoingFlows.FirstOrDefault(f => IsDefaultFlow(f));
        var defaultOption = defaultFlow != null ? (defaultFlow.Name ?? defaultFlow.Id) : "";
        
        if (!string.IsNullOrEmpty(defaultOption))
        {
            choices.Remove(defaultOption);
        }

        var selectedChoices = ShowMultiChoiceDialog("Inclusive Gateway", "Select one or more outgoing flows:", choices, defaultOption);
        if (selectedChoices.Count == 0) return;

        var originalElement = token.CurrentElement;
        bool first = true;
        foreach (var choice in selectedChoices)
        {
            var flow = outgoingFlows.FirstOrDefault(f => (f.Name ?? f.Id) == choice);
            if (flow == null) continue;

            var next = _tokenManager.GetTargetElement(flow);
            if (next == null) continue;

            if (_objectBounds.TryGetValue(next.Id, out var bounds))
            {
                var path = _paths.ContainsKey(flow.Id) ? _paths[flow.Id] : null;

                if (first)
                {
                    _tokenManager.SetTokenWaiting(token, false);
                    _tokenManager.MoveToken(token, next, flow, path, bounds);
                    first = false;
                }
                else
                {
                    var newToken = _tokenManager.AddToken(originalElement, bounds);
                    newToken.Parent = token.Parent;
                    _tokenManager.MoveToken(newToken, next, flow, path, bounds);
                }
            }
        }
    }
    
    private void HandleComplexGateway(BPMNToken token, ComplexGateway gateway, IEnumerable<SequenceFlow> outgoingFlows)
    {
        var choices = outgoingFlows
            .Select(f => f.Name ?? f.Id)
            .ToList();

        if (choices.Count == 0) return;

        var selectedChoices = ShowMultiChoiceDialog("Complex Gateway", "Select one or more outgoing flows (custom logic):", choices);
        if (selectedChoices.Count == 0) return;

        var originalElement = token.CurrentElement;
        bool first = true;
        foreach (var choice in selectedChoices)
        {
            var flow = outgoingFlows.FirstOrDefault(f => (f.Name ?? f.Id) == choice);
            if (flow == null) continue;

            var next = _tokenManager.GetTargetElement(flow);
            if (next == null) continue;

            if (_objectBounds.TryGetValue(next.Id, out var bounds))
            {
                var path = _paths.ContainsKey(flow.Id) ? _paths[flow.Id] : null;

                if (first)
                {
                    _tokenManager.MoveToken(token, next, flow, path, bounds);
                    first = false;
                }
                else
                {
                    var newToken = _tokenManager.AddToken(originalElement, bounds);
                    newToken.Parent = token.Parent;
                    _tokenManager.MoveToken(newToken, next, flow, path, bounds);
                }
            }
        }
    }
    
    private void HandleEventBasedGateway(BPMNToken token, EventBasedGateway gateway, IEnumerable<SequenceFlow> outgoingFlows)
    {
        var eventChoices = outgoingFlows
            .Select(f => f.Name ?? f.Id)
            .ToList();

        if (eventChoices.Count == 0) return;

        var chosenEvent = ShowChoiceDialog("Event-Based Gateway", "Which event occurred?", eventChoices);
        if (chosenEvent == null) return;

        var chosenFlow = outgoingFlows.FirstOrDefault(f => (f.Name ?? f.Id) == chosenEvent);
        if (chosenFlow == null) return;

        var next = _tokenManager.GetTargetElement(chosenFlow);
        if (next == null) return;

        if (_objectBounds.TryGetValue(next.Id, out var bounds))
        {
            var path = _paths.ContainsKey(chosenFlow.Id) ? _paths[chosenFlow.Id] : null;
            _tokenManager.MoveToken(token, next, chosenFlow, path, bounds);
        }
    }
    
    private bool IsDefaultFlow(SequenceFlow flow)
    {
        var sourceElement = flow.SourceRef;

        return sourceElement switch
        {
            Activity a => a.Default?.Id == flow.Id,
            ComplexGateway cg => cg.Default?.Id == flow.Id,
            ExclusiveGateway eg => eg.Default?.Id == flow.Id,
            InclusiveGateway ig => ig.Default?.Id == flow.Id,
            _ => false
        };
    }
}