using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;
using Task = BPMNModel.Model.Task;

namespace BPMNVisualizer.Visualization.Renderers;

public class ActivityRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;
    private readonly Dictionary<string, Rect> _objectBounds;
    private readonly Dictionary<string, FrameworkElement> _activityNotes = new();
    private readonly Dictionary<string, BPMNShape> _shapes;

    public ActivityRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager,
        SvgResourceManager svgResourceManager, Dictionary<string, Rect> objectBounds, Dictionary<string, BPMNShape> shapes)
    {
        _logger = logger;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
        _svgResourceManager = svgResourceManager;
        _objectBounds = objectBounds;
        _shapes = shapes;
    }

    public void RenderShape(BaseElement element, Rect bounds)
    {
        if (element is not Activity activity) return;
        
        _objectBounds[activity.Id] = bounds;

        var shape = DrawElement(activity, bounds);
        shape.MouseDown += (s, e) => ShowActivityDetails(activity);

        Canvas.SetLeft(shape, bounds.Left);
        Canvas.SetTop(shape, bounds.Top);
        _canvas.Children.Add(shape);

        if (activity is Task task)
        {
            var icon = DrawIcon(task, bounds);
            if (icon != null)
            {
                Canvas.SetLeft(icon, bounds.Left + bounds.Width * 0.05);
                Canvas.SetTop(icon, bounds.Top + bounds.Height * 0.05);
                _canvas.Children.Add(icon);
            }
        }

        var markers = DrawMarkers(activity, bounds);
        if (markers != null)
        {
            Canvas.SetLeft(markers, bounds.Left + (bounds.Width - markers.Width) / 2);
            Canvas.SetTop(markers, bounds.Top + bounds.Height * 0.9 - markers.Height);
            _canvas.Children.Add(markers);
        }

        var label = DrawLabel(activity, bounds);
        if (label != null)
        {
            Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
            Canvas.SetTop(label, bounds.Top + (bounds.Height - label.Height) / 2);
            _canvas.Children.Add(label);
        }
        
        var note = DrawNote(activity, bounds);
        if (note != null)
        {
            Canvas.SetLeft(note, bounds.Left + 5);
            Canvas.SetTop(note, bounds.Top - note.Height - 5);
            _canvas.Children.Add(note);
        }
    }

    private UIElement DrawElement(Activity activity, Rect bounds)
    {
        var shape = activity switch
        {
            Task => _shapeManager.GetRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            CallActivity =>
                _shapeManager.GetThickRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            Transaction =>
                _shapeManager.GetDoubleRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            SubProcess { TriggeredByEvent: true } =>
                _shapeManager.GetRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity), true),
            SubProcess =>
                _shapeManager.GetRoundedRectangle(bounds, _brushManager.GetActivityBrush(activity)),
            _ => throw new NotSupportedException($"Activity type {activity.GetType()} is not supported.")
        };
        return shape;
    }

    private Border? DrawIcon(Task task, Rect bounds)
    {
        var icon = _svgResourceManager.GetTaskIcon(task);
        if (icon == null)
        {
            _logger.Warning("No icon found for task type: {TaskType}", task.GetType());
            return null;
        }
        return _shapeManager.WrapInContainer(icon, bounds, 0.40);
    }

    private Border? DrawMarkers(Activity activity, Rect bounds)
    {
        var markers = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
        };

        if (activity.IsForCompensation == true || HasCompensationEvent(activity))
            AddMarker("Compensation", markers, bounds);

        if (activity.LoopCharacteristics != null)
        {
            switch (activity.LoopCharacteristics)
            {
                case MultiInstanceLoopCharacteristics mi:
                    var markerType = mi.IsSequential == true ? "Sequential-MultiInstance" : "Parallel-MultiInstance";
                    AddMarker(markerType, markers, bounds);
                    break;
                case StandardLoopCharacteristics:
                    AddMarker("Loop", markers, bounds);
                    break;
            }
        }

        _shapes.TryGetValue(activity.Id, out var bpmnShape);
        if (activity is SubProcess && bpmnShape?.IsExpanded == false)
            AddMarker("SubProcess", markers, bounds);
        if (activity is AdHocSubProcess) AddMarker("AdHoc", markers, bounds);

        return markers.Children.Count > 0 ? _shapeManager.WrapInContainer(markers, bounds) : null;
    }

    private bool HasCompensationEvent(Activity activity)
    {
        return activity.BoundaryEventRefs.Any(e => e.EventDefinitions.Any(d => d is CompensateEventDefinition));
    }

    private void AddMarker(string markerType, StackPanel container, Rect bounds)
    {
        var marker = _svgResourceManager.GetMarker(markerType);
        if (marker == null)
        {
            _logger.Warning("Marker icon not found: {MarkerType}", markerType);
            return;
        }

        double size = Math.Min(bounds.Width, bounds.Height) * 0.2;
        var viewbox = new Viewbox { Child = marker, Width = size, Height = size };
        container.Children.Add(viewbox);
    }

    private Border? DrawLabel(Activity activity, Rect bounds)
    {
        var text = activity.Name;
        if (string.IsNullOrEmpty(text))
        {
            _logger.Warning("No label found for activity type: {ActivityType}", activity.GetType());
            return null;
        }

        bounds.Width *= 0.9;
        var label = _shapeManager.GetLabel(text, bounds);
        label.VerticalAlignment = VerticalAlignment.Center;
        return _shapeManager.WrapInContainer(label, bounds);
    }
    
    private Border DrawNote(Activity activity, Rect bounds)
    {
        var noteText = ElementNotes.GetNote(activity.Id);
        if (string.IsNullOrEmpty(noteText)) return null;

        var border = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 64)),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(4, 2, 4, 2),
            Margin = new Thickness(2),
            MaxWidth = bounds.Width,
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

    private void ShowActivityDetails(Activity activity)
    {
        var detailWindow = new Window
        {
            Title = "Activity Details",
            Width = 700, 
            Height = 500,
            Content = CreateDetailContent(activity),
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        detailWindow.Show();
        detailWindow.Activate();
    }
    
    private UIElement CreateDetailContent(Activity activity)
    {
        var rootGrid = new Grid();
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Note
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Columns
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Script

        // ========== 1. Note Section ==========
        var notePanel = new StackPanel { Margin = new Thickness(10) };
        var noteTextBox = new TextBox
        {
            Text = ElementNotes.GetNote(activity.Id) ?? "",
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
            ElementNotes.SetNote(activity.Id, noteTextBox.Text);
            RefreshActivityVisual(activity);
        };
        notePanel.Children.Add(new TextBlock { Text = "Activity Note:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 2) });
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
        AddProperty(stdPanel, "ID", activity.Id ?? "null");
        AddProperty(stdPanel, "Name", activity.Name ?? "null");
        AddProperty(stdPanel, "Type", activity.GetType().Name);
        AddProperty(stdPanel, "Start Qty", activity.StartQuantity?.ToString());
        AddProperty(stdPanel, "Complete Qty", activity.CompletionQuantity?.ToString());
        AddProperty(stdPanel, "Compensation", activity.IsForCompensation?.ToString());
        
        // Loop
        if (activity.LoopCharacteristics != null)
            AddProperty(stdPanel, "Loop", FormatLoopInfo(activity.LoopCharacteristics));
            
        // Connections
        AddProperty(stdPanel, "Incoming", FormatConnections(activity.Incoming));
        AddProperty(stdPanel, "Outgoing", FormatConnections(activity.Outgoing));
        AddProperty(stdPanel, "Lanes", FormatLanes(activity.Lanes));
        
        // Data Handling
        if (activity.IoSpecification != null)
             AddProperty(stdPanel, "IO Spec", FormatIoSpecification(activity.IoSpecification));
        
        if (activity.DataInputAssociations.Any() || activity.DataOutputAssociations.Any())
            AddProperty(stdPanel, "Data Flow", FormatDataAssociations(activity));

        // Boundary Events
        if (activity.BoundaryEventRefs.Any())
            AddProperty(stdPanel, "Boundary Events", FormatBoundaryEvents(activity.BoundaryEventRefs));

        // Conversations
        if (activity is InteractionNode interactionNode)
             AddProperty(stdPanel, "Conversations", FormatConversations(interactionNode));

        // Extensions (Standard)
        if (activity.ExtensionDefinitions.Any() || activity.ExtensionValues.Any())
            AddProperty(stdPanel, "Extensions", FormatExtensions(activity));

        AddProperty(stdPanel, "Documentation", FormatDocumentation(activity.Documentation));


        // --- Camunda Properties ---
        AddProperty(camPanel, "Async Before", activity.Camunda_asyncBefore?.ToString());
        AddProperty(camPanel, "Async After", activity.Camunda_asyncAfter?.ToString());
        AddProperty(camPanel, "Exclusive", activity.Camunda_exclusive?.ToString());
        AddProperty(camPanel, "Job Priority", activity.Camunda_jobPriority);
        
        // Task Specifics
        AddCamundaTaskSpecifics(camPanel, activity);

        // Generic Camunda Elements
        AddProperty(camPanel, "Properties", FormatCamundaProperties(activity));
        AddProperty(camPanel, "Form Data", FormatCamundaFormData(activity));
        AddProperty(camPanel, "Input/Output", FormatCamundaIO(activity));
        AddProperty(camPanel, "Call Variables", FormatCamundaCallVars(activity)); // CamundaIn/Out
        AddProperty(camPanel, "Fields", FormatCamundaFields(activity));
        AddProperty(camPanel, "Connectors", FormatConnectors(activity));
        AddProperty(camPanel, "Listeners", FormatListenersSummary(activity)); // Classes/Delegates
        
        var retryCycle = activity.CamundaElements.OfType<CamundaFailedJobRetryTimeCycle>().FirstOrDefault();
        if (retryCycle != null) AddProperty(camPanel, "Retry Cycle", retryCycle.Body);

        Grid.SetColumn(stdPanel, 0);
        Grid.SetColumn(camPanel, 1);
        columnsGrid.Children.Add(stdPanel);
        columnsGrid.Children.Add(camPanel);

        var detailsScroll = new ScrollViewer { Content = columnsGrid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        Grid.SetRow(detailsScroll, 1);
        rootGrid.Children.Add(detailsScroll);

        // ========== 3. Script/Code Section ==========
        var scriptContent = ExtractScriptContent(activity);
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
    
    private void AddCamundaTaskSpecifics(StackPanel panel, Activity activity)
    {
        if (activity is UserTask ut)
        {
            AddProperty(panel, "Assignee", ut.Camunda_assignee);
            AddProperty(panel, "Cand. Users", ut.Camunda_candidateUsers);
            AddProperty(panel, "Cand. Groups", ut.Camunda_candidateGroups);
            AddProperty(panel, "Due Date", ut.Camunda_dueDate);
            AddProperty(panel, "Priority", ut.Camunda_priority);
            AddProperty(panel, "Form Key", ut.Camunda_formKey);
        }
        else if (activity is ServiceTask st)
        {
            AddProperty(panel, "Type", !string.IsNullOrEmpty(st.Camunda_type) ? st.Camunda_type : "Class/Delegate");
            AddProperty(panel, "Topic", st.Camunda_topic);
            AddProperty(panel, "Result Var", st.Camunda_resultVariable);
            if (!string.IsNullOrEmpty(st.Camunda_class)) AddProperty(panel, "Class", st.Camunda_class);
            if (!string.IsNullOrEmpty(st.Camunda_delegateExpression)) AddProperty(panel, "Delegate", st.Camunda_delegateExpression);
        }
        else if (activity is ScriptTask sct)
        {
            AddProperty(panel, "Format", sct.ScriptFormat);
            AddProperty(panel, "Resource", sct.Camunda_resource);
            AddProperty(panel, "Result Var", sct.Camunda_resultVariable);
        }
        else if (activity is CallActivity ca)
        {
            AddProperty(panel, "Called Elem", ca.CalledElementRef?.Id);
            AddProperty(panel, "Binding", ca.Camunda_calledElementBinding);
            AddProperty(panel, "Version", ca.Camunda_calledElementVersion);
            AddProperty(panel, "Tenant", ca.Camunda_calledElementTenantId);
            AddProperty(panel, "Case Ref", ca.Camunda_caseRef);
        }
        else if (activity is BusinessRuleTask brt)
        {
             AddProperty(panel, "Decision Ref", brt.Camunda_decisionRef);
             AddProperty(panel, "Binding", brt.Camunda_decisionRefBinding);
             AddProperty(panel, "Version", brt.Camunda_decisionRefVersion);
             AddProperty(panel, "Result Var", brt.Camunda_resultVariable);
             AddProperty(panel, "Map Result", brt.Camunda_mapDecisionResult);
             if (!string.IsNullOrEmpty(brt.Camunda_class)) AddProperty(panel, "Class", brt.Camunda_class);
             if (!string.IsNullOrEmpty(brt.Camunda_delegateExpression)) AddProperty(panel, "Delegate", brt.Camunda_delegateExpression);
        }
        else if (activity is SendTask send)
        {
             AddProperty(panel, "Message", send.MessageRef?.Name ?? "None");
             if (!string.IsNullOrEmpty(send.Camunda_class)) AddProperty(panel, "Class", send.Camunda_class);
             if (!string.IsNullOrEmpty(send.Camunda_delegateExpression)) AddProperty(panel, "Delegate", send.Camunda_delegateExpression);
        }
        else if (activity is ReceiveTask receive)
        {
             AddProperty(panel, "Message", receive.MessageRef?.Name ?? "None");
        }
    }

    // --- Data Extraction Helpers ---

    private string ExtractScriptContent(Activity activity)
    {
        var sb = new StringBuilder();

        // 1. Main Script Tasks
        if (activity is ScriptTask st && !string.IsNullOrEmpty(st.Script))
        {
            sb.AppendLine($"--- Main Script ({st.ScriptFormat}) ---");
            sb.AppendLine(st.Script);
            sb.AppendLine();
        }
        // 2. Service Tasks with Expressions
        else if (activity is ServiceTask srv && !string.IsNullOrEmpty(srv.Camunda_expression))
        {
            sb.AppendLine("--- Expression ---");
            sb.AppendLine(srv.Camunda_expression);
            sb.AppendLine();
        }

        // 3. Exec Listeners
        var execListeners = activity.CamundaElements.OfType<CamundaExecutionListener>();
        foreach (var l in execListeners)
        {
            if (l.Script != null && !string.IsNullOrEmpty(l.Script.Value))
            {
                sb.AppendLine($"--- Listener ({l.Event}): {l.Script.ScriptFormat} ---");
                sb.AppendLine(l.Script.Value);
                sb.AppendLine();
            }
        }

        // 4. Task Listeners
        var taskListeners = activity.CamundaElements.OfType<CamundaTaskListener>();
        foreach (var l in taskListeners)
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
    
    // Formats non-script listeners for the column view
    private string FormatListenersSummary(Activity activity)
    {
        var sb = new StringBuilder();
        var execListeners = activity.CamundaElements.OfType<CamundaExecutionListener>();
        foreach(var l in execListeners)
        {
            string impl = l.Class ?? l.DelegateExpression ?? l.Expression ?? (l.Script != null ? "Script" : "Unknown");
            sb.AppendLine($"Exec({l.Event}): {impl}");
        }
        
        var taskListeners = activity.CamundaElements.OfType<CamundaTaskListener>();
        foreach(var l in taskListeners)
        {
            string impl = l.Class ?? l.DelegateExpression ?? l.Expression ?? (l.Script != null ? "Script" : "Unknown");
            sb.AppendLine($"Task({l.Event}): {impl}");
        }
        return sb.ToString();
    }

    private string FormatCamundaProperties(Activity activity)
    {
        var props = activity.CamundaElements.OfType<CamundaProperty>();
        return props.Any() ? string.Join("\n", props.Select(p => $"{p.Name}: {p.Value}")) : "";
    }
    
    private string FormatCamundaFields(Activity activity)
    {
         var fields = activity.CamundaElements.OfType<CamundaField>();
         return fields.Any() ? string.Join("\n", fields.Select(f => $"{f.Name} = {f.StringValue ?? f.Expression}")) : "";
    }
    
    private string FormatCamundaFormData(Activity activity)
    {
        var formData = activity.CamundaElements.OfType<CamundaFormData>().FirstOrDefault();
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
    
    private string FormatCamundaIO(Activity activity)
    {
        var io = activity.CamundaElements.OfType<CamundaInputOutput>().FirstOrDefault();
        if (io == null) return "";
        var sb = new StringBuilder();
        foreach (var p in io.InputParameters) sb.AppendLine($"In: {p.Name} = {p.Value}");
        foreach (var p in io.OutputParameters) sb.AppendLine($"Out: {p.Name} = {p.Value}");
        return sb.ToString();
    }
    
    private string FormatCamundaCallVars(Activity activity)
    {
        var sb = new StringBuilder();
        var camundaIn = activity.CamundaElements.OfType<CamundaIn>();
        foreach(var v in camundaIn)
        {
            if (v.Variables == "all") sb.AppendLine("In: All");
            else if (v.BusinessKey != null) sb.AppendLine($"In: BusinessKey={v.BusinessKey}");
            else sb.AppendLine($"In: {v.Target} = {v.Source ?? v.SourceExpression}");
        }
        
        var camundaOut = activity.CamundaElements.OfType<CamundaOut>();
        foreach(var v in camundaOut)
        {
             if (v.Variables == "all") sb.AppendLine("Out: All");
             else sb.AppendLine($"Out: {v.Target} = {v.Source ?? v.SourceExpression}");
        }
        return sb.ToString();
    }
    
    private string FormatConnectors(Activity activity)
    {
         var conns = activity.CamundaElements.OfType<CamundaConnector>();
         return conns.Any() ? string.Join("\n", conns.Select(c => c.ConnectorId)) : "";
    }

    private string FormatConnections(IEnumerable<SequenceFlow> flows) => 
        string.Join(", ", flows.Select(f => f.Id));

    private string FormatLanes(IEnumerable<Lane> lanes) => 
        string.Join(", ", lanes.Select(l => l.Name ?? l.Id));

    private string FormatIoSpecification(InputOutputSpecification io)
    {
        var sb = new StringBuilder();
        if (io.DataInputs.Any()) sb.AppendLine("In: " + string.Join(", ", io.DataInputs.Select(i => i.Name)));
        if (io.DataOutputs.Any()) sb.AppendLine("Out: " + string.Join(", ", io.DataOutputs.Select(o => o.Name)));
        return sb.ToString().Trim();
    }

    private string FormatDataAssociations(Activity activity)
    {
        var sb = new StringBuilder();
        foreach (var assoc in activity.DataInputAssociations)
        {
            var src = string.Join(",", assoc.SourceRef.Select(s => (s as BaseElement)?.Id));
            var tgt = (assoc.TargetRef as BaseElement)?.Id ?? "null";
            sb.AppendLine($"{src} -> {tgt}");
        }
        foreach (var assoc in activity.DataOutputAssociations)
        {
            var src = string.Join(",", assoc.SourceRef.Select(s => (s as BaseElement)?.Id));
            var tgt = (assoc.TargetRef as BaseElement)?.Id ?? "null";
            sb.AppendLine($"{src} -> {tgt}");
        }
        return sb.ToString().Trim();
    }
    
    private string FormatBoundaryEvents(IEnumerable<BoundaryEvent> events)
    {
        return string.Join("\n", events.Select(e => 
            $"{e.Id} ({e.EventDefinitions.FirstOrDefault()?.GetType().Name.Replace("EventDefinition","")})"));
    }
    
    private string FormatConversations(InteractionNode node)
    {
        var sb = new StringBuilder();
        foreach(var c in node.IncomingConversationLinks)
            sb.AppendLine($"From: {(c.SourceRef as BaseElement)?.Id}");
        foreach(var c in node.OutgoingConversationLinks)
            sb.AppendLine($"To: {(c.TargetRef as BaseElement)?.Id}");
        return sb.ToString();
    }
    
    private string FormatExtensions(Activity activity)
    {
        var sb = new StringBuilder();
        foreach(var def in activity.ExtensionDefinitions) sb.AppendLine($"Def: {def.Name}");
        foreach(var val in activity.ExtensionValues) {
            var ValueText = val.Value?.ToString() ?? val.ValueRef?.Value ?? "null";
            sb.AppendLine($"Val: {val.ExtensionAttributeDefinition?.Name}: {ValueText}");
        }
        return sb.ToString();
    }

    private string FormatDocumentation(IEnumerable<Documentation> docs) =>
        string.Join("\n", docs.Select(d => d.Text));
    
    private string FormatLoopInfo(LoopCharacteristics? loop)
    {
        if (loop is MultiInstanceLoopCharacteristics mi)
        {
             var type = mi.IsSequential == true ? "Sequential" : "Parallel";
             var coll = mi.InputDataItem?.Name ?? "null";
             var cond = (mi.CompletionCondition as FormalExpression)?.Body;
             return $"{type}\nColl: {coll}" + (cond != null ? $"\nEnd: {cond}" : "");
        }
        if (loop is StandardLoopCharacteristics sl)
        {
             return $"Standard Loop\nTest Before: {sl.TestBefore}";
        }
        return "";
    }

    private void RefreshActivityVisual(Activity activity)
    {
        RefreshActivityNote(activity);
        if (_activityNotes.TryGetValue(activity.Id, out var note)) Panel.SetZIndex(note, int.MaxValue);
    }
    
    private void RefreshActivityNote(Activity activity)
    {
        if (_activityNotes.TryGetValue(activity.Id, out var existingNote)) _canvas.Children.Remove(existingNote);
        var note = DrawNote(activity, _objectBounds[activity.Id]);
        if (note != null)
        {
            note.Tag = $"{activity.Id}_note";
            _activityNotes[activity.Id] = note;
            Canvas.SetLeft(note, _objectBounds[activity.Id].Left + 5);
            Canvas.SetTop(note, _objectBounds[activity.Id].Top - 20);
            _canvas.Children.Add(note);
        }
    }
}