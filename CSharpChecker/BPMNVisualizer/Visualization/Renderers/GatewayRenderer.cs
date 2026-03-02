using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;

namespace BPMNVisualizer.Visualization.Renderers;

public class GatewayRenderer : IShapeRenderer
{
    private readonly SharedVariables _vars;
    /*
    private readonly ILogger _logger;
    private readonly Dictionary<string, Rect> _vars.ObjectBounds;
*/
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;
    
    private readonly Dictionary<string, FrameworkElement> _gatewayNotes = new();
    
    private readonly bool _isHistoryMode;

    public GatewayRenderer(Canvas canvas, BrushManager brushManager, ShapeManager shapeManager, SvgResourceManager svgResourceManager, bool isHistoryMode = false)
    {
        _vars = SharedVariables.Instance;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
        _svgResourceManager = svgResourceManager;
        _isHistoryMode = isHistoryMode;
    }

    public void RenderShape(BaseElement element, Rect bounds)
    {
        if (element is not Gateway gateway) return;

        var shape = DrawElement(gateway, bounds);

        if (!_isHistoryMode)
        {
            _vars.ObjectBounds[gateway.Id] = bounds;
            shape.MouseDown += (s, e) => ShowGatewayDetails(gateway);
        }

        Canvas.SetLeft(shape, bounds.Left);
        Canvas.SetTop(shape, bounds.Top);
        _canvas.Children.Add(shape);

        var icon = DrawIcon(gateway, bounds);
        if (icon != null)
        {
            Canvas.SetLeft(icon, bounds.Left + (bounds.Width - icon.Width) / 2);
            Canvas.SetTop(icon, bounds.Top + (bounds.Height - icon.Height) / 2);
            if (!_isHistoryMode)
                icon.MouseDown += (s, e) => ShowGatewayDetails(gateway);
            _canvas.Children.Add(icon);
        }

        var label = DrawLabel(gateway, bounds);
        if (label != null)
        {
            Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
            Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
            if (!_isHistoryMode)
                label.MouseDown += (s, e) => ShowGatewayDetails(gateway);
            _canvas.Children.Add(label);
        }
        
        if (_isHistoryMode) return;
        
        var note = DrawNote(gateway, bounds);
        if (note != null)
        {
            Canvas.SetLeft(note, bounds.Left + 5);
            Canvas.SetTop(note, bounds.Top - note.Height - 5);
            note.MouseDown += (s, e) => ShowGatewayDetails(gateway);
            _canvas.Children.Add(note);
        }
    }

    private UIElement DrawElement(Gateway gateway, Rect bounds) => _shapeManager.GetDiamond(bounds, _brushManager.GetGatewayBrush(gateway));
    
    private Border? DrawIcon(Gateway gateway, Rect bounds)
    {
        var icon = _svgResourceManager.GetGatewayIcon(gateway);
        if (icon == null)
        {
            _vars.Logger.Warning("No icon found for gateway type: {GatewayType}", gateway.GetType());
            return null;
        }

        return _shapeManager.WrapInContainer(icon, bounds, 0.25);
    }

    private Border? DrawLabel(Gateway gateway, Rect bounds)
    {
        var text = gateway.Name;
        if (string.IsNullOrEmpty(text))
        {
            _vars.Logger.Warning("No label found for gateway type: {GatewayType}", gateway.GetType());
            return null;
        }

        bounds.Width *= 2;
        var label = _shapeManager.GetLabel(text, bounds);

        return _shapeManager.WrapInContainer(label, bounds);
    }
    
    private Border DrawNote(Gateway gateway, Rect bounds)
    {
        var noteText = ElementNotes.GetNote(gateway.Id);
        if (string.IsNullOrEmpty(noteText)) return null;

        var border = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 64)),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(4, 2, 4, 2),
            Margin = new Thickness(2),
            MaxWidth = bounds.Width * 2,
            Child = new TextBlock
            {
                Text = noteText,
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.DarkSlateGray,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis
            }
        };

        border.LayoutUpdated += (s, e) => 
        {
            var actualHeight = border.ActualHeight;
            Canvas.SetTop(border, bounds.Top - actualHeight - 5);
        };

        Canvas.SetLeft(border, bounds.Left + 5);
        Canvas.SetTop(border, bounds.Top);

        border.Measure(new Size(bounds.Width, double.PositiveInfinity));
        border.Arrange(new Rect(border.DesiredSize));

        return border;
    }
    
    private void ShowGatewayDetails(Gateway gateway)
    {
        var detailWindow = new DetailWindow
        {
            Owner = Application.Current.MainWindow,
            Title = "Gateway Details"
        };
        
        detailWindow.SetNote("Gateway Note:", ElementNotes.GetNote(gateway.Id) ?? "", (text) => 
        {
            ElementNotes.SetNote(gateway.Id, text);
            RefreshGatewayVisual(gateway);
        });

        // --- Standard Properties ---
        detailWindow.AddStandardProperty("ID", gateway.Id ?? "null");
        detailWindow.AddStandardProperty("Name", gateway.Name ?? "null");
        detailWindow.AddStandardProperty("Type", gateway.GetType().Name);
        detailWindow.AddStandardProperty("Direction", gateway.GatewayDirection?.ToString());

        // Gateway Specific Logic
        switch (gateway)
        {
            case ExclusiveGateway exclusive:
                detailWindow.AddStandardProperty("Default Flow", exclusive.Default?.Id ?? "None");
                break;
            case InclusiveGateway inclusive:
                detailWindow.AddStandardProperty("Default Flow", inclusive.Default?.Id ?? "None");
                break;
            case EventBasedGateway eventGateway:
                detailWindow.AddStandardProperty("Instantiate", eventGateway.Instantiate?.ToString());
                detailWindow.AddStandardProperty("Event Type", eventGateway.EventGatewayType?.ToString());
                break;
            case ComplexGateway complex:
                string actCond = "Null";
                if (complex.ActivationCondition is FormalExpression fe) actCond = fe.Body.Value;
                else if (complex.ActivationCondition != null) actCond = complex.ActivationCondition.ToString();
                
                detailWindow.AddStandardProperty("Activation", actCond);
                detailWindow.AddStandardProperty("Default Flow", complex.Default?.Id ?? "None");
                break;
            case ParallelGateway parallel:
                var syncType = parallel.GatewayDirection switch
                {
                    GatewayDirection.Converging => "Join (Synchronization)",
                    GatewayDirection.Diverging => "Split (Fork)",
                    _ => "Mixed/Unknown"
                };
                detailWindow.AddStandardProperty("Sync Type", syncType);
                break;
        }

        detailWindow.AddStandardProperty("Incoming", FormatConnections(gateway.Incoming));
        detailWindow.AddStandardProperty("Outgoing", FormatConnections(gateway.Outgoing));
        detailWindow.AddStandardProperty("Lanes", FormatLanes(gateway.Lanes));
        
        // Flow Analysis
        detailWindow.AddStandardProperty("Flow Analysis", FormatFlowAnalysis(gateway));

        // Extensions (Standard)
        if (gateway.ExtensionDefinitions.Any() || gateway.ExtensionValues.Any())
            detailWindow.AddStandardProperty("Extensions", FormatExtensions(gateway));

        detailWindow.AddStandardProperty("Docs", FormatDocumentation(gateway.Documentation));

        // --- Camunda Properties ---
        detailWindow.AddCamundaProperty("Async Before", gateway.Camunda_asyncBefore?.ToString());
        detailWindow.AddCamundaProperty("Async After", gateway.Camunda_asyncAfter?.ToString());
        detailWindow.AddCamundaProperty("Exclusive", gateway.Camunda_exclusive?.ToString());
        detailWindow.AddCamundaProperty("Job Priority", gateway.Camunda_jobPriority);
        
        // Generic Camunda Elements
        detailWindow.AddCamundaProperty("Properties", FormatCamundaProperties(gateway));
        detailWindow.AddCamundaProperty("Input/Output", FormatCamundaIO(gateway));
        detailWindow.AddCamundaProperty("Fields", FormatCamundaFields(gateway));
        detailWindow.AddCamundaProperty("Connectors", FormatConnectors(gateway));
        detailWindow.AddCamundaProperty("Listeners", FormatListenersSummary(gateway)); 

        // ========== 3. Script/Code Section ==========
        var scriptContent = ExtractScriptContent(gateway);
        detailWindow.SetScript(scriptContent);
        
        detailWindow.Show();
        detailWindow.Activate();
    }
    
    // --- Data Extraction Helpers ---

    private string ExtractScriptContent(Gateway gateway)
    {
        var sb = new StringBuilder();
        
        // 1. Flow Conditions
        foreach(var flow in gateway.Outgoing)
        {
            if (flow.ConditionExpression is FormalExpression expr && !string.IsNullOrEmpty(expr.Body.Value))
            {
                sb.AppendLine($"--- Flow Condition ({flow.Id}) ---");
                sb.AppendLine(expr.Body.Value.Trim());
                sb.AppendLine();
            }
        }
        
        // 2. Listeners
        var listeners = gateway.CamundaElements.OfType<CamundaExecutionListener>();
        foreach (var l in listeners)
        {
            if (l.Script != null && !string.IsNullOrEmpty(l.Script.Value))
            {
                sb.AppendLine($"--- Listener ({l.Event}): {l.Script.ScriptFormat} ---");
                sb.AppendLine(l.Script.Value);
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }
    
    private string FormatFlowAnalysis(Gateway gateway)
    {
        var sb = new StringBuilder();
        var defaultFlow = GetDefaultFlow(gateway);

        foreach (var flow in gateway.Outgoing)
        {
            sb.Append($"• {flow.Id}: ");
            
            if (flow == defaultFlow)
            {
                sb.AppendLine("[DEFAULT]");
                continue;
            }

            if (flow.ConditionExpression is FormalExpression expr)
            {
                var body = expr.Body?.Value.Trim() ?? "Null";
                if (body.Length > 40) body = body.Substring(0, 40) + "..."; // Truncate for column
                sb.AppendLine($"\"{body}\"");
            }
            else if (gateway is InclusiveGateway)
            {
                sb.AppendLine("(Implicit/No Condition)");
            }
            else
            {
                sb.AppendLine("No Condition");
            }
        }
        return sb.ToString();
    }
    
    private SequenceFlow? GetDefaultFlow(Gateway gateway)
    {
        return gateway switch
        {
            ExclusiveGateway ex => ex.Default,
            InclusiveGateway inc => inc.Default,
            ComplexGateway cmp => cmp.Default,
            _ => null
        };
    }

    private string FormatListenersSummary(Gateway gateway)
    {
        var sb = new StringBuilder();
        var listeners = gateway.CamundaElements.OfType<CamundaExecutionListener>();
        foreach(var l in listeners)
        {
            string type = "Unknown";
            string val = "";

            if (l.Class != null) { type = "Class"; val = l.Class; }
            else if (l.DelegateExpression != null) { type = "Delegate"; val = l.DelegateExpression; }
            else if (l.Expression != null) { type = "Expr"; val = l.Expression; }
            else if (l.Script != null) { type = "Script"; val = l.Script.ScriptFormat ?? "Script"; }

            sb.AppendLine($"{l.Event}: [{type}] {val}");
        }
        return sb.ToString();
    }

    private string FormatCamundaProperties(Gateway gateway)
    {
        var props = gateway.CamundaElements.OfType<CamundaProperty>();
        return props.Any() ? string.Join("\n", props.Select(p => $"{p.Name}: {p.Value}")) : "";
    }
    
    private string FormatCamundaFields(Gateway gateway)
    {
         var fields = gateway.CamundaElements.OfType<CamundaField>();
         return fields.Any() ? string.Join("\n", fields.Select(f => $"{f.Name} = {f.StringValue ?? f.Expression}")) : "";
    }
    
    private string FormatCamundaIO(Gateway gateway)
    {
        var io = gateway.CamundaElements.OfType<CamundaInputOutput>().FirstOrDefault();
        if (io == null) return "";
        var sb = new StringBuilder();
        foreach (var p in io.InputParameters) sb.AppendLine($"In: {p.Name} = {p.Value}");
        foreach (var p in io.OutputParameters) sb.AppendLine($"Out: {p.Name} = {p.Value}");
        return sb.ToString();
    }
    
    private string FormatConnectors(Gateway gateway)
    {
         var conns = gateway.CamundaElements.OfType<CamundaConnector>();
         if (!conns.Any()) return "";

         var sb = new StringBuilder();
         foreach (var conn in conns)
         {
             sb.AppendLine($"ID: {conn.ConnectorId}");
             if (conn.InputOutput != null)
             {
                 foreach (var p in conn.InputOutput.InputParameters)
                     sb.AppendLine($"  In: {p.Name} = {p.Value}");
                 foreach (var p in conn.InputOutput.OutputParameters)
                     sb.AppendLine($"  Out: {p.Name} = {p.Value}");
             }
         }
         return sb.ToString();
    }

    private string FormatConnections(IEnumerable<SequenceFlow> flows) => 
        string.Join(", ", flows.Select(f => f.Id));

    private string FormatLanes(IEnumerable<Lane> lanes) => 
        string.Join(", ", lanes.Select(l => l.Name ?? l.Id));
        
    private string FormatExtensions(Gateway gateway)
    {
        var sb = new StringBuilder();
        foreach(var def in gateway.ExtensionDefinitions) sb.AppendLine($"Def: {def.Name}");
        foreach(var val in gateway.ExtensionValues) {
            var ValueText = val.Value?.ToString() ?? val.ValueRef?.Value ?? "null";
            sb.AppendLine($"Val: {val.ExtensionAttributeDefinition?.Name}: {ValueText}");
        }
        return sb.ToString();
    }
    
    private string FormatDocumentation(IEnumerable<Documentation> docs) =>
        string.Join("\n", docs.Select(d => d.Text));

    private void RefreshGatewayVisual(Gateway gateway)
    {
        RefreshGatewayNote(gateway);
        if (_gatewayNotes.TryGetValue(gateway.Id, out var note)) Panel.SetZIndex(note, int.MaxValue);
    }
    
    private void RefreshGatewayNote(Gateway gateway)
    {
        if (_gatewayNotes.TryGetValue(gateway.Id, out var existingNote)) _canvas.Children.Remove(existingNote);
        var note = DrawNote(gateway, _vars.ObjectBounds[gateway.Id]);
        if (note != null) {
            note.Tag = $"{gateway.Id}_note";
            _gatewayNotes[gateway.Id] = note;
            Canvas.SetLeft(note, _vars.ObjectBounds[gateway.Id].Left + 5);
            Canvas.SetTop(note, _vars.ObjectBounds[gateway.Id].Top - 20);
            _canvas.Children.Add(note);
        }
    }
}