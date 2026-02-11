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

public class EventRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;
    private readonly Dictionary<string, Rect> _objectBounds;
    private readonly Dictionary<string, FrameworkElement> _eventNotes = new();

    public EventRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager,
        SvgResourceManager svgResourceManager, Dictionary<string, Rect> objectBounds)
    {
        _logger = logger;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
        _svgResourceManager = svgResourceManager;
        _objectBounds = objectBounds;
    }

    public void RenderShape(BaseElement element, Rect bounds)
    {
        if (element is not Event evt) return;
        
        _objectBounds[evt.Id] = bounds;

        var shape = DrawElement(evt, bounds);
        shape.MouseDown += (s, e) => ShowEventDetails(evt);

        Canvas.SetLeft(shape, bounds.Left);
        Canvas.SetTop(shape, bounds.Top);
        _canvas.Children.Add(shape);

        var icon = DrawIcon(evt, bounds);
        if (icon != null)
        {
            Canvas.SetLeft(icon, bounds.Left + (bounds.Width - icon.Width) / 2);
            Canvas.SetTop(icon, bounds.Top + (bounds.Height - icon.Height) / 2);
            _canvas.Children.Add(icon);
        }

        var label = DrawLabel(evt, bounds);
        if (label != null)
        {
            Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
            Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
            _canvas.Children.Add(label);
        }
        
        var note = DrawNote(evt, bounds);
        if (note != null)
        {
            Canvas.SetLeft(note, bounds.Left + 5);
            Canvas.SetTop(note, bounds.Top - note.Height - 5);
            _canvas.Children.Add(note);
        }
    }

    private UIElement DrawElement(Event evt, Rect bounds)
    {
        return evt switch
        {
            StartEvent start =>
                _shapeManager.GetCircle(bounds, _brushManager.GetEventBrush(evt), start.IsInterrupting == false),
            IntermediateCatchEvent or IntermediateThrowEvent =>
                _shapeManager.GetDoubleCircle(bounds, _brushManager.GetEventBrush(evt)),
            BoundaryEvent boundary =>
                _shapeManager.GetDoubleCircle(bounds, _brushManager.GetEventBrush(evt),
                    boundary.CancelActivity == false),
            EndEvent =>
                _shapeManager.GetThickCircle(bounds, _brushManager.GetEventBrush(evt)),
            _ =>
                _shapeManager.GetCircle(bounds, _brushManager.GetEventBrush(evt))
        };
    }

    private Border? DrawIcon(Event evt, Rect bounds)
    {
        var icon = _svgResourceManager.GetEventIcon(evt);
        if (icon == null)
        {
            _logger.Warning("No icon found for event type: {EventType}", evt.GetType());
            return null;
        }

        return _shapeManager.WrapInContainer(icon, bounds, 0.25);
    }

    private Border? DrawLabel(Event evt, Rect bounds)
    {
        var text = evt.Name;
        if (string.IsNullOrEmpty(text))
        {
            // Some events don't have labels, suppressing warning to reduce noise
            return null;
        }

        bounds.Width *= 2;
        var label = _shapeManager.GetLabel(text, bounds);

        return _shapeManager.WrapInContainer(label, bounds);
    }
    
    private Border DrawNote(Event evt, Rect bounds)
    {
        var noteText = ElementNotes.GetNote(evt.Id);
        if (string.IsNullOrEmpty(noteText)) return null;

        var border = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 64)),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(4, 2, 4, 2),
            Margin = new Thickness(2),
            MaxWidth = bounds.Width * 3,
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

    private void ShowEventDetails(Event evt)
    {
        var detailWindow = new Window
        {
            Title = "Event Details",
            Width = 700,
            Height = 500,
            Content = CreateDetailContent(evt),
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        detailWindow.Show();
        detailWindow.Activate();
    }

    private UIElement CreateDetailContent(Event evt)
    {
        var rootGrid = new Grid();
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Note
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Columns
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Script

        // ========== 1. Note Section ==========
        var notePanel = new StackPanel { Margin = new Thickness(10) };
        var noteTextBox = new TextBox
        {
            Text = ElementNotes.GetNote(evt.Id) ?? "",
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Height = 40,
            Margin = new Thickness(0, 0, 0, 5)
        };
        var saveButton = new Button
        {
            Content = "Save Note",
            Margin = new Thickness(0, 5, 0, 5),
            Padding = new Thickness(5)
        };
        saveButton.Click += (s, e) => 
        {
            ElementNotes.SetNote(evt.Id, noteTextBox.Text);
            RefreshEventVisual(evt);
        };
        notePanel.Children.Add(new TextBlock { Text = "Event Note:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 2) });
        notePanel.Children.Add(noteTextBox);
        notePanel.Children.Add(saveButton);
        
        Grid.SetRow(notePanel, 0);
        rootGrid.Children.Add(notePanel);

        // ========== 2. Columns Section (Standard & Camunda) ==========
        var columnsGrid = new Grid();
        columnsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        columnsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        
        var stdPanel = new StackPanel { Margin = new Thickness(10, 0, 10, 0) };
        var camPanel = new StackPanel { Margin = new Thickness(10, 0, 10, 0) };

        // --- Standard Properties ---
        AddProperty(stdPanel, "ID", evt.Id ?? "null");
        AddProperty(stdPanel, "Name", evt.Name ?? "null");
        AddProperty(stdPanel, "Type", evt.GetType().Name);
        
        // Flow
        AddProperty(stdPanel, "Incoming", FormatConnections(evt.Incoming));
        AddProperty(stdPanel, "Outgoing", FormatConnections(evt.Outgoing));
        AddProperty(stdPanel, "Lanes", FormatLanes(evt.Lanes));

        // Event Specifics (Start/Boundary)
        if (evt is StartEvent start) 
            AddProperty(stdPanel, "Interrupting", start.IsInterrupting?.ToString());
        if (evt is BoundaryEvent boundary) {
            AddProperty(stdPanel, "Cancel Act", boundary.CancelActivity?.ToString());
            AddProperty(stdPanel, "Attached To", boundary.AttachedToRef?.Id);
        }

        // Definitions & Data Flow
        AddProperty(stdPanel, "Definitions", FormatEventDefinitions(evt));
        AddProperty(stdPanel, "Data Flow", FormatDataAssociations(evt));
        AddProperty(stdPanel, "Docs", FormatDocumentation(evt.Documentation));
        
        // Extensions (Standard)
        if (evt.ExtensionDefinitions.Any() || evt.ExtensionValues.Any())
            AddProperty(stdPanel, "Extensions", FormatExtensions(evt));


        // --- Camunda Properties ---
        AddProperty(camPanel, "Async Before", evt.Camunda_asyncBefore?.ToString());
        AddProperty(camPanel, "Async After", evt.Camunda_asyncAfter?.ToString());
        AddProperty(camPanel, "Job Priority", evt.Camunda_jobPriority);
        
        if (evt is StartEvent se)
        {
            AddProperty(camPanel, "Form Key", se.Camunda_formKey);
            AddProperty(camPanel, "Initiator", se.Camunda_initiator);
        }

        // Generic Camunda Elements
        AddProperty(camPanel, "Properties", FormatCamundaProperties(evt));
        AddProperty(camPanel, "Form Data", FormatCamundaFormData(evt));
        AddProperty(camPanel, "Input/Output", FormatCamundaIO(evt));
        AddProperty(camPanel, "Fields", FormatCamundaFields(evt));
        AddProperty(camPanel, "Connectors", FormatConnectors(evt));
        AddProperty(camPanel, "Listeners", FormatListenersSummary(evt)); // Non-script listener info

        var retryCycle = evt.CamundaElements.OfType<CamundaFailedJobRetryTimeCycle>().FirstOrDefault();
        if (retryCycle != null) AddProperty(camPanel, "Retry Cycle", retryCycle.Body);

        Grid.SetColumn(stdPanel, 0);
        Grid.SetColumn(camPanel, 1);
        columnsGrid.Children.Add(stdPanel);
        columnsGrid.Children.Add(camPanel);

        var detailsScroll = new ScrollViewer { Content = columnsGrid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        Grid.SetRow(detailsScroll, 1);
        rootGrid.Children.Add(detailsScroll);

        // ========== 3. Script/Code Section ==========
        var scriptContent = ExtractScriptContent(evt);
        if (!string.IsNullOrWhiteSpace(scriptContent))
        {
            var scriptPanel = new StackPanel { Margin = new Thickness(10) };
            scriptPanel.Children.Add(new TextBlock { Text = "Script / Code / Expressions:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 2) });
            
            var scriptBox = new TextBox
            {
                Text = scriptContent,
                IsReadOnly = true,
                FontFamily = new FontFamily("Consolas"),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 150,
                AcceptsReturn = true
            };
            scriptPanel.Children.Add(scriptBox);
            
            Grid.SetRow(scriptPanel, 2);
            rootGrid.Children.Add(scriptPanel);
        }

        return new Border { Padding = new Thickness(5), Child = rootGrid };
    }

    private void AddProperty(StackPanel panel, string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        var grid = new Grid { Margin = new Thickness(0, 2, 0, 2) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var lbl = new TextBlock 
        { 
            Text = $"{label}:", 
            FontWeight = FontWeights.Bold, 
            VerticalAlignment = VerticalAlignment.Top,
            TextWrapping = TextWrapping.Wrap
        };
        var val = new TextBlock 
        { 
            Text = value, 
            FontFamily = new FontFamily("Consolas"), 
            TextWrapping = TextWrapping.Wrap, 
            VerticalAlignment = VerticalAlignment.Top 
        };

        Grid.SetColumn(lbl, 0);
        Grid.SetColumn(val, 1);
        grid.Children.Add(lbl);
        grid.Children.Add(val);
        panel.Children.Add(grid);
    }
    
    // --- Data Extraction Helpers ---

    private string ExtractScriptContent(Event evt)
    {
        var sb = new StringBuilder();
        var listeners = evt.CamundaElements.OfType<CamundaExecutionListener>();
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
    
    private string FormatEventDefinitions(Event evt)
    {
        IEnumerable<EventDefinition> eventDefinitions = Enumerable.Empty<EventDefinition>();
        if (evt is CatchEvent ce) eventDefinitions = ce.EventDefinitions;
        else if (evt is ThrowEvent te) eventDefinitions = te.EventDefinitions;

        if (!eventDefinitions.Any()) return "";

        var sb = new StringBuilder();
        foreach(var def in eventDefinitions)
        {
             var type = def.GetType().Name.Replace("EventDefinition", "");
             sb.Append($"• {type}");
             
             if (def is MessageEventDefinition msg && msg.MessageRef != null)
                sb.Append($" (Ref: {msg.MessageRef.Name ?? msg.MessageRef.Id})");
             else if (def is SignalEventDefinition sig && sig.SignalRef != null)
                sb.Append($" (Ref: {sig.SignalRef.Name ?? sig.SignalRef.Id})");
             else if (def is ErrorEventDefinition err)
             {
                 if (err.ErrorRef != null) sb.Append($" (Ref: {err.ErrorRef.Name ?? err.ErrorRef.Id})");
                 if (!string.IsNullOrEmpty(err.Camunda_errorCodeVariable)) sb.Append($"\n  [ErrCodeVar: {err.Camunda_errorCodeVariable}]");
                 if (!string.IsNullOrEmpty(err.Camunda_errorMessageVariable)) sb.Append($"\n  [ErrMsgVar: {err.Camunda_errorMessageVariable}]");
             }
             sb.AppendLine();
        }
        return sb.ToString().Trim();
    }
    
    private string FormatDataAssociations(Event evt)
    {
        var sb = new StringBuilder();

        if (evt is ThrowEvent throwEvent && throwEvent.DataInputAssociation.Any())
        {
            sb.AppendLine("Input Mappings:");
            foreach (var assoc in throwEvent.DataInputAssociation)
            {
                var src = assoc.SourceRef.Any() ? string.Join(", ", assoc.SourceRef.Select(s => (s as BaseElement)?.Id)) : "None";
                var tgt = (assoc.TargetRef as BaseElement)?.Id ?? "None";
                var transform = assoc.Transformation != null ? " [Has Transform]" : "";
                sb.AppendLine($"• {src} → {tgt}{transform}");
            }
        }

        if (evt is CatchEvent catchEvt && catchEvt.DataOutputAssociation.Any())
        {
            sb.AppendLine("Output Mappings:");
            foreach (var assoc in catchEvt.DataOutputAssociation)
            {
                var src = assoc.SourceRef.Any() ? string.Join(", ", assoc.SourceRef.Select(s => (s as BaseElement)?.Id)) : "None";
                var tgt = (assoc.TargetRef as BaseElement)?.Id ?? "None";
                sb.AppendLine($"• {src} → {tgt}");
            }
        }
        return sb.ToString().Trim();
    }

    private string FormatListenersSummary(Event evt)
    {
        var sb = new StringBuilder();
        var listeners = evt.CamundaElements.OfType<CamundaExecutionListener>();
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

    private string FormatCamundaProperties(Event evt)
    {
        var props = evt.CamundaElements.OfType<CamundaProperty>();
        return props.Any() ? string.Join("\n", props.Select(p => $"{p.Name}: {p.Value}")) : "";
    }
    
    private string FormatCamundaFields(Event evt)
    {
         var fields = evt.CamundaElements.OfType<CamundaField>();
         return fields.Any() ? string.Join("\n", fields.Select(f => $"{f.Name} = {f.StringValue ?? f.Expression}")) : "";
    }
    
    private string FormatCamundaFormData(Event evt)
    {
        var formData = evt.CamundaElements.OfType<CamundaFormData>().FirstOrDefault();
        if (formData == null || !formData.Fields.Any()) return "";
        
        var sb = new StringBuilder();
        foreach (var f in formData.Fields)
        {
             var label = !string.IsNullOrEmpty(f.Label) ? $"\"{f.Label}\"" : f.Id;
             var val = !string.IsNullOrEmpty(f.DefaultValue) ? $"={f.DefaultValue}" : "";
             sb.AppendLine($"• {label} ({f.Type}){val}");
        }
        return sb.ToString();
    }
    
    private string FormatCamundaIO(Event evt)
    {
        var io = evt.CamundaElements.OfType<CamundaInputOutput>().FirstOrDefault();
        if (io == null) return "";
        var sb = new StringBuilder();
        foreach (var p in io.InputParameters) sb.AppendLine($"In: {p.Name} = {p.Value}");
        foreach (var p in io.OutputParameters) sb.AppendLine($"Out: {p.Name} = {p.Value}");
        return sb.ToString();
    }
    
    private string FormatConnectors(Event evt)
    {
         var conns = evt.CamundaElements.OfType<CamundaConnector>();
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
        
    private string FormatExtensions(Event evt)
    {
        var sb = new StringBuilder();
        foreach(var def in evt.ExtensionDefinitions) sb.AppendLine($"Def: {def.Name}");
        foreach(var val in evt.ExtensionValues) {
            var ValueText = val.Value?.ToString() ?? val.ValueRef?.Value ?? "null";
            sb.AppendLine($"Val: {val.ExtensionAttributeDefinition?.Name}: {ValueText}");
        }
        return sb.ToString();
    }
    
    private string FormatDocumentation(IEnumerable<Documentation> docs) =>
        string.Join("\n", docs.Select(d => d.Text));

    private void RefreshEventVisual(Event evt)
    {
        RefreshEventNote(evt);
        if (_eventNotes.TryGetValue(evt.Id, out var note)) Panel.SetZIndex(note, int.MaxValue);
    }
    
    private void RefreshEventNote(Event evt)
    {
        if (_eventNotes.TryGetValue(evt.Id, out var existingNote)) _canvas.Children.Remove(existingNote);
        var note = DrawNote(evt, _objectBounds[evt.Id]);
        if (note != null) {
            note.Tag = $"{evt.Id}_note";
            _eventNotes[evt.Id] = note;
            Canvas.SetLeft(note, _objectBounds[evt.Id].Left + 5);
            Canvas.SetTop(note, _objectBounds[evt.Id].Top - 20);
            _canvas.Children.Add(note);
        }
    }
}