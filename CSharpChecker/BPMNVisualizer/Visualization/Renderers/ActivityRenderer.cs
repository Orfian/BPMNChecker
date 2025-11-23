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
        {
            AddMarker("Compensation", markers, bounds);
        }

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
        if (activity is SubProcess)
        {
            if (bpmnShape?.IsExpanded == false)
            {
                AddMarker("SubProcess", markers, bounds);
            }
        }
        if (activity is AdHocSubProcess) AddMarker("AdHoc", markers, bounds);

        return markers.Children.Count > 0 ? _shapeManager.WrapInContainer(markers, bounds) : null;
    }

    private bool HasCompensationEvent(Activity activity)
    {
        return activity.BoundaryEventRefs.Any(e =>
            e.EventDefinitions.Any(d => d is CompensateEventDefinition));
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
        var viewbox = new Viewbox
        {
            Child = marker,
            Width = size,
            Height = size
        };

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
            Width = 600,
            Height = 400,
            Content = CreateDetailContent(activity),
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        detailWindow.Show();
        detailWindow.Activate();
    }
    
    private UIElement CreateDetailContent(Activity activity)
    {
        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // ========== Note Section ==========
        var notePanel = new StackPanel { Margin = new Thickness(10) };
    
        var noteTextBox = new TextBox
        {
            Text = ElementNotes.GetNote(activity.Id) ?? "",
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Height = 30,
            Margin = new Thickness(0, 0, 0, 5)
        };
    
        var saveButton = new Button
        {
            Content = "Save Note",
            Margin = new Thickness(0, 5, 0, 10),
            Padding = new Thickness(5)
        };
    
        saveButton.Click += (s, e) => 
        {
            ElementNotes.SetNote(activity.Id, noteTextBox.Text);
            RefreshActivityVisual(activity);
        };
    
        notePanel.Children.Add(new TextBlock { 
            Text = "Activity Note:", 
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        });
        notePanel.Children.Add(noteTextBox);
        notePanel.Children.Add(saveButton);

        Grid.SetRow(notePanel, 0);
        mainGrid.Children.Add(notePanel);

        // ========== Details Section ==========
        var detailsScroll = new ScrollViewer
        {
            Content = CreateDetailsGrid(activity),
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        Grid.SetRow(detailsScroll, 1);
        mainGrid.Children.Add(detailsScroll);

        return new Border
        {
            Padding = new Thickness(10),
            Child = mainGrid
        };
    }

    private Grid CreateDetailsGrid(Activity activity)
    {
        var noteGrid = new Grid();
        noteGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        noteGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
    
        // ========== Note Section ==========
        var notePanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };
    
        var noteTextBox = new TextBox
        {
            Text = ElementNotes.GetNote(activity.Id) ?? "",
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Height = 60,
            Margin = new Thickness(5)
        };
    
        var saveButton = new Button
        {
            Content = "Save Note",
            Margin = new Thickness(5),
            Padding = new Thickness(5, 2, 5, 2)
        };
    
        saveButton.Click += (s, e) => 
        {
            ElementNotes.SetNote(activity.Id, noteTextBox.Text);
            RefreshActivityVisual(activity);
        };
    
        notePanel.Children.Add(new TextBlock { Text = "Note:", Margin = new Thickness(5, 0, 5, 2) });
        notePanel.Children.Add(noteTextBox);
        notePanel.Children.Add(saveButton);
    
        Grid.SetRow(notePanel, 0);
        noteGrid.Children.Add(notePanel);
        
        // ========== Details Section ==========
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        int rowIndex = 0;

        // ========== Core Properties ==========
        AddDetailRow(grid, "ID:", activity.Id ?? "null", ref rowIndex);
        AddDetailRow(grid, "Name:", activity.Name ?? "null", ref rowIndex);
        AddDetailRow(grid, "Type:", activity.GetType().Name, ref rowIndex);

        // ========== Activity-Specific Properties ==========
        AddDetailRow(grid, "Compensation:", activity.IsForCompensation?.ToString() ?? "null", ref rowIndex);
        AddDetailRow(grid, "Start Qty:", activity.StartQuantity?.ToString() ?? "null", ref rowIndex);
        AddDetailRow(grid, "Complete Qty:", activity.CompletionQuantity?.ToString() ?? "null", ref rowIndex);

        // ========== Flow Relationships ==========
        AddDetailRow(grid, "Incoming:", FormatConnections(activity.Incoming), ref rowIndex);
        AddDetailRow(grid, "Outgoing:", FormatConnections(activity.Outgoing), ref rowIndex);
        AddDetailRow(grid, "Lanes:", FormatLanes(activity.Lanes), ref rowIndex);

        // ========== Task-Specific Properties ==========
        if (activity is Task task)
        {
            AddDetailRow(grid, "Type Specific:", GetTaskSpecificInfo(task), ref rowIndex);
        }

        // ========== Loop Configuration ==========
        if (activity.LoopCharacteristics != null)
        {
            AddDetailRow(grid, "Loop:", FormatLoopInfo(activity.LoopCharacteristics), ref rowIndex);
        }

        // ========== Data Handling ==========
        var ioInfo = new StringBuilder();
        if (activity.IoSpecification != null)
        {
            ioInfo.AppendLine(FormatIoSpecification(activity.IoSpecification));
        }

        ioInfo.Append(FormatDataAssociations(activity));
        AddDetailRow(grid, "Data Flow:", ioInfo.ToString(), ref rowIndex);

        // ========== Boundary Events ==========
        if (activity.BoundaryEventRefs.Any())
        {
            AddDetailRow(grid, "Boundary Events:", FormatBoundaryEvents(activity.BoundaryEventRefs), ref rowIndex);
        }

        // ========== Conversation Links ==========
        if (activity is InteractionNode interactionNode)
        {
            var conversationInfo = new StringBuilder();

            if (interactionNode.IncomingConversationLinks.Any())
            {
                conversationInfo.AppendLine("Incoming Conversations:");
                foreach (var conv in interactionNode.IncomingConversationLinks)
                {
                    conversationInfo.AppendLine($"• From: {(conv.SourceRef != null ? GetElementDisplay(conv.SourceRef) : "null")}");
                }
            }

            if (interactionNode.OutgoingConversationLinks.Any())
            {
                conversationInfo.AppendLine("Outgoing Conversations:");
                foreach (var conv in interactionNode.OutgoingConversationLinks)
                {
                    conversationInfo.AppendLine($"• To: {(conv.TargetRef != null ? GetElementDisplay(conv.TargetRef) : "null")}");
                }
            }

            if (conversationInfo.Length > 0)
            {
                AddDetailRow(grid, "Conversations:", conversationInfo.ToString(), ref rowIndex);
            }
        }

        // ========== Documentation ==========
        if (activity.Documentation.Any())
        {
            AddDetailRow(grid, "Docs:", FormatDocumentation(activity.Documentation), ref rowIndex);
        }

        // ========== Camunda Extensions ==========
        var camundaInfo = new StringBuilder();

        var camundaProps = activity.CamundaElements
            .OfType<CamundaProperty>()
            .ToList();

        if (camundaProps.Any())
        {
            camundaInfo.AppendLine("Properties:");
            foreach (var prop in camundaProps)
            {
                camundaInfo.AppendLine($"• {prop.Name}: {prop.Value}");
            }
        }

        // Execution Listeners
        var listeners = activity.CamundaElements
            .OfType<CamundaExecutionListener>();
        if (listeners.Any())
        {
            camundaInfo.AppendLine("\nExecution Listeners:");
            foreach (var listener in listeners)
            {
                camundaInfo.AppendLine($"• {listener.Event}: {listener.Class ?? listener.Expression}");
            }
        }

        // Connectors
        var connectors = activity.CamundaElements
            .OfType<CamundaConnector>()
            .ToList();

        if (connectors.Any())
        {
            camundaInfo.AppendLine("\nConnectors:");
            foreach (var conn in connectors)
            {
                camundaInfo.AppendLine($"• Connector ID: {conn.ConnectorId ?? "N/A"}");

                if (conn.InputOutput != null)
                {
                    // Input Parameters
                    if (conn.InputOutput.InputParameters.Any())
                    {
                        camundaInfo.AppendLine("  Input Parameters:");
                        foreach (var param in conn.InputOutput.InputParameters)
                        {
                            var value = param.Value ?? "null";
                            camundaInfo.AppendLine($"  - {param.Name} = {value}");
                        }
                    }

                    // Output Parameters
                    if (conn.InputOutput.OutputParameters.Any())
                    {
                        camundaInfo.AppendLine("  Output Parameters:");
                        foreach (var param in conn.InputOutput.OutputParameters)
                        {
                            var value = param.Value ?? "null";
                            camundaInfo.AppendLine($"  - {param.Name} = {value}");
                        }
                    }
                }
            }
        }

        switch (activity)
        {
            case UserTask userTask:
                if (!string.IsNullOrEmpty(userTask.Camunda_assignee))
                    camundaInfo.AppendLine($"• Assignee: {userTask.Camunda_assignee}");
                if (!string.IsNullOrEmpty(userTask.Camunda_candidateUsers))
                    camundaInfo.AppendLine($"• Candidate Users: {userTask.Camunda_candidateUsers}");
                if (!string.IsNullOrEmpty(userTask.Camunda_formKey))
                    camundaInfo.AppendLine($"• Form Key: {userTask.Camunda_formKey}");
                break;

            case ServiceTask serviceTask:
                if (!string.IsNullOrEmpty(serviceTask.Camunda_class))
                    camundaInfo.AppendLine($"• Delegate Class: {serviceTask.Camunda_class}");
                if (!string.IsNullOrEmpty(serviceTask.Camunda_expression))
                    camundaInfo.AppendLine($"• Expression: {serviceTask.Camunda_expression}");
                break;

            case CallActivity callActivity:
                if (!string.IsNullOrEmpty(callActivity.Camunda_calledElementBinding))
                    camundaInfo.AppendLine($"• Binding: {callActivity.Camunda_calledElementBinding}");

                if (!string.IsNullOrEmpty(callActivity.Camunda_calledElementVersion))
                    camundaInfo.AppendLine($"• Version: {callActivity.Camunda_calledElementVersion}");

                if (!string.IsNullOrEmpty(callActivity.Camunda_calledElementVersionTag))
                    camundaInfo.AppendLine($"• Version Tag: {callActivity.Camunda_calledElementVersionTag}");

                if (!string.IsNullOrEmpty(callActivity.Camunda_calledElementTenantId))
                    camundaInfo.AppendLine($"• Tenant ID: {callActivity.Camunda_calledElementTenantId}");

                if (!string.IsNullOrEmpty(callActivity.Camunda_caseRef))
                    camundaInfo.AppendLine($"• Case Ref: {callActivity.Camunda_caseRef}");

                if (!string.IsNullOrEmpty(callActivity.Camunda_variableMappingClass))
                    camundaInfo.AppendLine($"• Mapping Class: {callActivity.Camunda_variableMappingClass}");

                break;
        }

        if (camundaInfo.Length > 0)
        {
            AddDetailRow(grid, "Camunda Properties:", camundaInfo.ToString(), ref rowIndex);
        }

        // ========== Extensions ==========
        if (activity.ExtensionDefinitions.Any() || activity.ExtensionValues.Any())
        {
            var extensionInfo = new StringBuilder();

            // Extension Definitions
            if (activity.ExtensionDefinitions.Any())
            {
                extensionInfo.AppendLine("Defined Extensions:");
                foreach (var def in activity.ExtensionDefinitions)
                {
                    extensionInfo.AppendLine($"• {def.Name}");
                    foreach (var attr in def.ExtensionAttributeDefinitions)
                    {
                        extensionInfo.AppendLine($"  - {attr.Name} ({attr.Type})");
                    }
                }
            }

            // Extension Values
            if (activity.ExtensionValues.Any())
            {
                extensionInfo.AppendLine("Applied Extensions:");
                foreach (var val in activity.ExtensionValues)
                {
                    var value = val.Value ?? val.ValueRef;
                    extensionInfo.AppendLine($"• {val.ExtensionAttributeDefinition?.Name}: {(value is null? "" :value.Value)}");
                }
            }

            AddDetailRow(grid, "Extensions:", extensionInfo.ToString(), ref rowIndex);
        }

        return grid;
    }

    // ========== Helper Methods ==========
    private string FormatConnections(IEnumerable<SequenceFlow> flows)
    {
        return flows.Any()
            ? string.Join("\n", flows.Select(f => $"• {f.Id}"))
            : "";
    }

    private string FormatLanes(IEnumerable<Lane> lanes)
    {
        return lanes.Any()
            ? string.Join("\n", lanes.Select(l => $"• {l.Name ?? l.Id}"))
            : "";
    }

    private string GetTaskSpecificInfo(Task task)
    {
        return task switch
        {
            UserTask ut =>
                $"User Task\n• Assignee: {ut.Camunda_assignee ?? "null"}\n• Form Key: {ut.Camunda_formKey ?? "null"}",
            ServiceTask st =>
                $"Service Task\n• Type: {st.Camunda_type ?? "null"}\n• Topic: {st.Camunda_topic ?? "null"}",
            ScriptTask st => $"Script Task\n• Format: {st.ScriptFormat ?? "null"}",
            _ => task.GetType().Name
        };
    }

    private string FormatLoopInfo(LoopCharacteristics loop)
    {
        return loop switch
        {
            MultiInstanceLoopCharacteristics mi =>
                $"Multi-Instance ({(mi.IsSequential == true ? "Sequential" : "Parallel")})\n" +
                $"• Collection: {mi.InputDataItem?.Name ?? "null"}\n" +
                $"• Completion: {(mi.CompletionCondition as FormalExpression)?.Body}",

            StandardLoopCharacteristics sl =>
                $"Standard Loop\n" +
                $"• Test Before: {sl.TestBefore}\n" +
                $"• Condition: {(sl.LoopCondition as FormalExpression)?.Body}",

            _ => "Custom Loop"
        };
    }

    private string FormatIoSpecification(InputOutputSpecification io)
    {
        var sb = new StringBuilder();
        if (io.DataInputs.Any())
        {
            sb.AppendLine("Inputs:");
            foreach (var input in io.DataInputs)
            {
                sb.AppendLine($"• {input.Name} {(input.IsCollection == true ? "(Collection)" : "")}");
            }
        }

        if (io.DataOutputs.Any())
        {
            sb.AppendLine("Outputs:");
            foreach (var output in io.DataOutputs)
            {
                sb.AppendLine($"• {output.Name} {(output.IsCollection == true ? "(Collection)" : "")}");
            }
        }

        return sb.ToString();
    }

    private string FormatDataAssociations(Activity activity)
    {
        var sb = new StringBuilder();

        if (activity.DataInputAssociations.Any())
        {
            sb.AppendLine("Input Mappings:");
            foreach (var assoc in activity.DataInputAssociations)
            {
                sb.AppendLine($"• Sources: {string.Join(", ", assoc.SourceRef.Select(GetElementId))}");
                sb.AppendLine($"  → Target: {(assoc.TargetRef is null ? "null" : GetElementId(assoc.TargetRef))}");
            }
        }

        if (activity.DataOutputAssociations.Any())
        {
            sb.AppendLine("Output Mappings:");
            foreach (var assoc in activity.DataOutputAssociations)
            {
                sb.AppendLine($"• Sources: {string.Join(", ", assoc.SourceRef.Select(GetElementId))}");
                sb.AppendLine($"  → Target: {(assoc.TargetRef is null ? "null" : GetElementId(assoc.TargetRef))}");
            }
        }

        return sb.ToString();
    }

    private string GetElementId(object element)
    {
        return (element as BaseElement)?.Id ?? "Anonymous";
    }

    private string FormatBoundaryEvents(IEnumerable<BoundaryEvent> events)
    {
        return string.Join("\n\n", events.Select(e =>
            $"• {e.Id}\n" +
            $"  Type: {e.EventDefinitions.FirstOrDefault()?.GetType().Name.Replace("EventDefinition", "")}\n" +
            $"  Cancel: {e.CancelActivity}"));
    }

    private string FormatDocumentation(IEnumerable<Documentation> docs)
    {
        return string.Join("\n\n", docs.Select(d =>
            $"• {d.Text} {(d.TextFormat != null ? $"[{d.TextFormat}]" : "")}"));
    }

    private string GetElementDisplay(InteractionNode node)
    {
        if (node is BaseElement baseElement)
            return $"{baseElement.Id} ({node.GetType().Name})";
        return "Unknown";
    }

    private void AddDetailRow(Grid grid, string label, object value, ref int row)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString())) return;

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var labelBlock = new TextBlock
        {
            Text = label,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(5, 2, 5, 2),
            VerticalAlignment = VerticalAlignment.Top
        };
        Grid.SetRow(labelBlock, row);
        Grid.SetColumn(labelBlock, 0);

        var valueBlock = new TextBlock
        {
            Text = value?.ToString() ?? "null",
            Margin = new Thickness(5, 2, 5, 2),
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("Consolas")
        };
        Grid.SetRow(valueBlock, row);
        Grid.SetColumn(valueBlock, 1);

        grid.Children.Add(labelBlock);
        grid.Children.Add(valueBlock);

        row++;
    }
    
    private void RefreshActivityVisual(Activity activity)
    {
        RefreshActivityNote(activity);
        
        if (_activityNotes.TryGetValue(activity.Id, out var note))
        {
            Panel.SetZIndex(note, int.MaxValue);
        }
    }
    
    private void RefreshActivityNote(Activity activity)
    {
        if (_activityNotes.TryGetValue(activity.Id, out var existingNote))
        {
            _canvas.Children.Remove(existingNote);
        }

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