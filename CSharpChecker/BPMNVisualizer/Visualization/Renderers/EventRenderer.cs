using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;

namespace BPMNVisualizer.Visualization.Renderers
{
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
                Width = 600,
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
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // ========== Note Section ==========
            var notePanel = new StackPanel { Margin = new Thickness(10) };
    
            var noteTextBox = new TextBox
            {
                Text = ElementNotes.GetNote(evt.Id) ?? "",
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 50,
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
                ElementNotes.SetNote(evt.Id, noteTextBox.Text);
                RefreshEventVisual(evt);
            };
    
            notePanel.Children.Add(new TextBlock { 
                Text = "Event Note:", 
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
                Content = CreateDetailsGrid(evt),
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

        private Grid CreateDetailsGrid(Event evt)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int rowIndex = 0;
            
            IEnumerable<EventDefinition> eventDefinitions = Enumerable.Empty<EventDefinition>();
            if (evt is CatchEvent ce) eventDefinitions = ce.EventDefinitions;
            else if (evt is ThrowEvent te) eventDefinitions = te.EventDefinitions;

            // ========== Core Event Properties ==========
            AddDetailRow(grid, "ID:", evt.Id ?? "null", ref rowIndex);
            AddDetailRow(grid, "Name:", evt.Name ?? "null", ref rowIndex);
            AddDetailRow(grid, "Type:", evt.GetType().Name, ref rowIndex);

            // ========== Event-Specific Properties ==========
            if (evt is StartEvent startEvent)
            {
                AddDetailRow(grid, "Interrupting:", startEvent.IsInterrupting?.ToString() ?? "null", ref rowIndex);
            }
            else if (evt is BoundaryEvent boundaryEvent)
            {
                AddDetailRow(grid, "Cancel Activity:", boundaryEvent.CancelActivity?.ToString() ?? "null", ref rowIndex);
                AddDetailRow(grid, "Attached To:", boundaryEvent.AttachedToRef?.Id ?? "null", ref rowIndex);
            }

            // ========== Event Definitions (Detailed) ==========
            if (eventDefinitions.Any())
            {
                 var definitionsInfo = new StringBuilder();
                 foreach(var def in eventDefinitions)
                 {
                     var type = def.GetType().Name.Replace("EventDefinition", "");
                     definitionsInfo.Append($"• {type}");
                     
                     // Add specific names/IDs if available
                     if (def is MessageEventDefinition msg && msg.MessageRef != null)
                        definitionsInfo.Append($" (Ref: {msg.MessageRef.Name ?? msg.MessageRef.Id})");
                     else if (def is SignalEventDefinition sig && sig.SignalRef != null)
                        definitionsInfo.Append($" (Ref: {sig.SignalRef.Name ?? sig.SignalRef.Id})");
                     else if (def is ErrorEventDefinition err && err.ErrorRef != null)
                        definitionsInfo.Append($" (Ref: {err.ErrorRef.Name ?? err.ErrorRef.Id})");
                     
                     definitionsInfo.AppendLine();
                 }
                 AddDetailRow(grid, "Event Definitions:", definitionsInfo.ToString(), ref rowIndex);
            }

            // ========== Flow Relationships ==========
            AddDetailRow(grid, "Incoming:", FormatConnections(evt.Incoming), ref rowIndex);
            AddDetailRow(grid, "Outgoing:", FormatConnections(evt.Outgoing), ref rowIndex);
            AddDetailRow(grid, "Lanes:", FormatLanes(evt.Lanes), ref rowIndex);

            // ========== Data Associations ==========
            var dataInfo = new StringBuilder();

            if (evt is ThrowEvent throwEvent && throwEvent.DataInputAssociation.Any())
            {
                dataInfo.AppendLine("Input Mappings:");
                foreach (var assoc in throwEvent.DataInputAssociation)
                {
                    dataInfo.AppendLine($"• {FormatDataAssociation(assoc)}");
                }
            }

            if (evt is CatchEvent catchEvt && catchEvt.DataOutputAssociation.Any())
            {
                dataInfo.AppendLine("Output Mappings:");
                foreach (var assoc in catchEvt.DataOutputAssociation)
                {
                    dataInfo.AppendLine($"• {FormatDataAssociation(assoc)}");
                }
            }

            if (dataInfo.Length > 0)
            {
                AddDetailRow(grid, "Data Flow:", dataInfo.ToString(), ref rowIndex);
            }

            // ========== Documentation ==========
            if (evt.Documentation.Any())
            {
                AddDetailRow(grid, "Docs:", FormatDocumentation(evt.Documentation), ref rowIndex);
            }

            // =========================================================
            // ========== CAMUNDA EXTENSIONS & ATTRIBUTES ==============
            // =========================================================
            var camundaInfo = new StringBuilder();

            // 1. General Camunda Attributes on Event
            if (evt.Camunda_asyncBefore.HasValue) camundaInfo.AppendLine($"• Async Before: {evt.Camunda_asyncBefore}");
            if (evt.Camunda_asyncAfter.HasValue) camundaInfo.AppendLine($"• Async After: {evt.Camunda_asyncAfter}");
            if (!string.IsNullOrEmpty(evt.Camunda_jobPriority)) camundaInfo.AppendLine($"• Job Priority: {evt.Camunda_jobPriority}");
            
            // Event Specific Attributes
            if (evt is StartEvent se)
            {
                 if (!string.IsNullOrEmpty(se.Camunda_formKey)) camundaInfo.AppendLine($"• Form Key: {se.Camunda_formKey}");
                 if (!string.IsNullOrEmpty(se.Camunda_initiator)) camundaInfo.AppendLine($"• Initiator: {se.Camunda_initiator}");
            }
            
            // Error Event Specifics
             var errorDef = eventDefinitions.OfType<ErrorEventDefinition>().FirstOrDefault();
             if (errorDef != null)
             {
                 if (!string.IsNullOrEmpty(errorDef.Camunda_errorCodeVariable)) 
                    camundaInfo.AppendLine($"• Error Code Var: {errorDef.Camunda_errorCodeVariable}");
                 if (!string.IsNullOrEmpty(errorDef.Camunda_errorMessageVariable)) 
                    camundaInfo.AppendLine($"• Error Msg Var: {errorDef.Camunda_errorMessageVariable}");
             }

            // 2. Camunda Properties (Key/Value pairs)
            var camundaProps = evt.CamundaElements.OfType<CamundaProperty>().ToList();
            if (camundaProps.Any())
            {
                camundaInfo.AppendLine("\n[Extension Properties]");
                foreach (var prop in camundaProps)
                    camundaInfo.AppendLine($"  {prop.Name}: {prop.Value}");
            }

            // 3. Form Data (Start Events)
            var formData = evt.CamundaElements.OfType<CamundaFormData>().FirstOrDefault();
            if (formData != null && formData.Fields.Any())
            {
                camundaInfo.AppendLine("\n[Form Data]");
                foreach (var field in formData.Fields)
                {
                    var label = !string.IsNullOrEmpty(field.Label) ? $"\"{field.Label}\"" : field.Id;
                    var type = !string.IsNullOrEmpty(field.Type) ? $" ({field.Type})" : "";
                    var def = !string.IsNullOrEmpty(field.DefaultValue) ? $" = {field.DefaultValue}" : "";
                    camundaInfo.AppendLine($"  {label}{type}{def}");
                }
            }

            // 4. Input/Output Mappings
            var inputOutput = evt.CamundaElements.OfType<CamundaInputOutput>().FirstOrDefault();
            if (inputOutput != null)
            {
                if (inputOutput.InputParameters.Any())
                {
                    camundaInfo.AppendLine("\n[Input Parameters]");
                    foreach (var p in inputOutput.InputParameters)
                        camundaInfo.AppendLine($"  {p.Name} = {p.Value}");
                }
                if (inputOutput.OutputParameters.Any())
                {
                    camundaInfo.AppendLine("\n[Output Parameters]");
                    foreach (var p in inputOutput.OutputParameters)
                        camundaInfo.AppendLine($"  {p.Name} = {p.Value}");
                }
            }

            // 5. Execution Listeners
            var listeners = evt.CamundaElements.OfType<CamundaExecutionListener>();
            if (listeners.Any())
            {
                camundaInfo.AppendLine("\n[Execution Listeners]");
                foreach (var l in listeners)
                {
                    string details = "Unknown Implementation";

                    if (!string.IsNullOrEmpty(l.Class)) 
                        details = $"Class: {l.Class}";
                    else if (!string.IsNullOrEmpty(l.Expression)) 
                        details = $"Expr: {l.Expression}";
                    else if (!string.IsNullOrEmpty(l.DelegateExpression)) 
                        details = $"Delegate: {l.DelegateExpression}";
                    else if (l.Script != null)
                    {
                        var scriptContent = l.Script.Value ?? "";
                        var preview = scriptContent.Trim().Replace("\n", " ");
                        if (preview.Length > 40) preview = preview.Substring(0, 40) + "...";
                        details = $"Script ({l.Script.ScriptFormat}): {preview}";
                    }

                    camundaInfo.AppendLine($"  {l.Event}: {details}");
                }
            }
            
            // 6. Field Injections
            var fields = evt.CamundaElements.OfType<CamundaField>();
            if (fields.Any())
            {
                camundaInfo.AppendLine("\n[Field Injections]");
                foreach (var field in fields)
                {
                    var val = field.StringValue ?? field.Expression ?? "null";
                    camundaInfo.AppendLine($"  {field.Name} = {val}");
                }
            }

            // 7. Connectors
            var connectors = evt.CamundaElements.OfType<CamundaConnector>();
            if (connectors.Any())
            {
                camundaInfo.AppendLine("\n[Connectors]");
                foreach (var conn in connectors)
                {
                    camundaInfo.AppendLine($"  ID: {conn.ConnectorId}");
                    if (conn.InputOutput != null)
                    {
                        foreach (var p in conn.InputOutput.InputParameters) camundaInfo.AppendLine($"    In: {p.Name} = {p.Value}");
                        foreach (var p in conn.InputOutput.OutputParameters) camundaInfo.AppendLine($"    Out: {p.Name} = {p.Value}");
                    }
                }
            }
            
            // 8. Retry Cycle
            var retryCycle = evt.CamundaElements.OfType<CamundaFailedJobRetryTimeCycle>().FirstOrDefault();
            if (retryCycle != null)
            {
                 camundaInfo.AppendLine($"\n[Retry Cycle] {retryCycle.Body}");
            }

            if (camundaInfo.Length > 0)
            {
                AddDetailRow(grid, "Camunda Config:", camundaInfo.ToString(), ref rowIndex);
            }

            // ========== Standard Extensions ==========
            if (evt.ExtensionDefinitions.Any() || evt.ExtensionValues.Any())
            {
                var extensionInfo = new StringBuilder();

                if (evt.ExtensionDefinitions.Any())
                {
                    extensionInfo.AppendLine("Definitions:");
                    foreach (var def in evt.ExtensionDefinitions)
                        extensionInfo.AppendLine($"• {def.Name}");
                }

                if (evt.ExtensionValues.Any())
                {
                    extensionInfo.AppendLine("Values:");
                    foreach (var val in evt.ExtensionValues)
                    {
                         var value = val.Value ?? val.ValueRef;
                        extensionInfo.AppendLine($"• {val.ExtensionAttributeDefinition?.Name}: {(value is null ? "" : value.Value)}");
                    }
                }

                AddDetailRow(grid, "Extensions:", extensionInfo.ToString(), ref rowIndex);
            }

            return grid;
        }

        // ========== Helper Methods ==========
        private string FormatDataAssociation(DataAssociation association)
        {
            var sources = association.SourceRef.Any()
                ? string.Join(", ", association.SourceRef.Select(GetElementId))
                : "No sources";

            var target = association.TargetRef != null
                ? GetElementId(association.TargetRef)
                : "No target";

            var transform = association.Transformation != null
                ? $" [Transform: {(association.Transformation as FormalExpression)?.Body}]"
                : "";

            return $"{sources} → {target}{transform}";
        }

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

        private string GetElementId(object element)
        {
            return (element as BaseElement)?.Id ?? "Anonymous";
        }

        private string FormatDocumentation(IEnumerable<Documentation> docs)
        {
            return string.Join("\n\n", docs.Select(d =>
                $"• {d.Text} {(d.TextFormat != null ? $"[{d.TextFormat}]" : "")}"));
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
        
        private void RefreshEventVisual(Event evt)
        {
            RefreshEventNote(evt);
        
            if (_eventNotes.TryGetValue(evt.Id, out var note))
            {
                Panel.SetZIndex(note, int.MaxValue);
            }
        }
    
        private void RefreshEventNote(Event evt)
        {
            if (_eventNotes.TryGetValue(evt.Id, out var existingNote))
            {
                _canvas.Children.Remove(existingNote);
            }

            var note = DrawNote(evt, _objectBounds[evt.Id]);
            if (note != null)
            {
                note.Tag = $"{evt.Id}_note";
                _eventNotes[evt.Id] = note;
            
                Canvas.SetLeft(note, _objectBounds[evt.Id].Left + 5);
                Canvas.SetTop(note, _objectBounds[evt.Id].Top - 20);
                _canvas.Children.Add(note);
            }
        }
    }
}