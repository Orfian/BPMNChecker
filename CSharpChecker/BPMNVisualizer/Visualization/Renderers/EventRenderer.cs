using System.Text;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Camunda;
using BPMNVisualizer.Utilities;
using Task = BPMNModel.Model.Task;

namespace BPMNVisualizer.Visualization.Renderers
{
    public class EventRenderer : IShapeRenderer
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly BrushManager _brushManager;
        private readonly ShapeManager _shapeManager;
        private readonly SvgResourceManager _svgResourceManager;

        public EventRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager,
            SvgResourceManager svgResourceManager)
        {
            _logger = logger;
            _canvas = canvas;
            _brushManager = brushManager;
            _shapeManager = shapeManager;
            _svgResourceManager = svgResourceManager;
        }

        public void RenderShape(BaseElement element, Rect bounds)
        {
            if (element is not Event evt) return;

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
                _logger.Warning("No label found for event type: {EventType}", evt.GetType());
                return null;
            }

            bounds.Width *= 2;
            var label = _shapeManager.GetLabel(text, bounds);

            return _shapeManager.WrapInContainer(label, bounds);
        }

        private void ShowEventDetails(Event evt)
        {
            var detailWindow = new Window
            {
                Title = "Event Details",
                Width = 600,
                Height = 400,
                Content = CreateDetailContent(evt),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            detailWindow.Show();
        }

        private UIElement CreateDetailContent(Event evt)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int rowIndex = 0;

            // ========== Core Event Properties ==========
            AddDetailRow(grid, "ID:", evt.Id ?? "null", ref rowIndex);
            AddDetailRow(grid, "Name:", evt.Name ?? "null", ref rowIndex);
            AddDetailRow(grid, "Type:", evt.GetType().Name, ref rowIndex);

            // ========== Event-Specific Properties ==========
            if (evt is StartEvent startEvent)
            {
                AddDetailRow(grid, "Interrupting:", startEvent.IsInterrupting?.ToString(), ref rowIndex);
            }
            else if (evt is BoundaryEvent boundaryEvent)
            {
                AddDetailRow(grid, "Cancel Activity:", boundaryEvent.CancelActivity?.ToString(), ref rowIndex);
                AddDetailRow(grid, "Attached To:", boundaryEvent.AttachedToRef?.Id ?? "null", ref rowIndex);
            }

            // ========== Event Definitions ==========
            if (evt is CatchEvent catchEvent && catchEvent.EventDefinitionRefs.Any())
            {
                AddDetailRow(grid, "Event Type:",
                    string.Join(", ", catchEvent.EventDefinitionRefs.Select(GetEventDefinitionType)),
                    ref rowIndex);
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

// ========== Camunda Extensions ==========
            var camundaInfo = new StringBuilder();

// General Camunda properties
            if (evt.Camunda_asyncBefore.HasValue)
                camundaInfo.AppendLine($"• Async Before: {evt.Camunda_asyncBefore}");
            if (evt.Camunda_asyncAfter.HasValue)
                camundaInfo.AppendLine($"• Async After: {evt.Camunda_asyncAfter}");
            if (!string.IsNullOrEmpty(evt.Camunda_jobPriority))
                camundaInfo.AppendLine($"• Job Priority: {evt.Camunda_jobPriority}");

// Camunda elements
            var camundaElements = evt.CamundaElements;

// Camunda Properties
            var camundaProps = camundaElements
                .OfType<CamundaProperties>()
                .SelectMany(p => p.Values)
                .ToList();

            if (camundaProps.Any())
            {
                camundaInfo.AppendLine("\nProperties:");
                foreach (var prop in camundaProps)
                {
                    camundaInfo.AppendLine($"• {prop.Name}: {prop.Value}");
                }
            }

// Execution Listeners
            var listeners = camundaElements
                .OfType<CamundaExecutionListener>()
                .ToList();

            if (listeners.Any())
            {
                camundaInfo.AppendLine("\nExecution Listeners:");
                foreach (var listener in listeners)
                {
                    var details = listener.Class != null
                        ? $"Class: {listener.Class}"
                        : $"Expression: {listener.Expression}";
                    camundaInfo.AppendLine($"• {listener.Event}: {details}");
                }
            }

// Connectors
            var connectors = camundaElements
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

            // Event-specific Camunda properties
            switch (evt)
            {
                case StartEvent sEvent:
                    if (!string.IsNullOrEmpty(sEvent.Camunda_formKey))
                        camundaInfo.AppendLine($"• Form Key: {sEvent.Camunda_formKey}");
                    if (!string.IsNullOrEmpty(sEvent.Camunda_initiator))
                        camundaInfo.AppendLine($"• Initiator: {sEvent.Camunda_initiator}");
                    break;

                case BoundaryEvent boundaryEvent:
                    var errorEvent = boundaryEvent.EventDefinitions
                        .OfType<ErrorEventDefinition>()
                        .FirstOrDefault();
                    if (errorEvent?.Camunda_errorCodeVariable != null)
                    {
                        camundaInfo.AppendLine($"• Error Code Variable: {errorEvent.Camunda_errorCodeVariable}");
                    }

                    break;
            }

            if (camundaInfo.Length > 0)
            {
                AddDetailRow(grid, "Camunda Properties:", camundaInfo.ToString(), ref rowIndex);
            }

            // ========== General Extensions ==========
            var extensionInfo = new StringBuilder();

            // Extension Definitions
            if (evt.ExtensionDefinitions.Any())
            {
                extensionInfo.AppendLine("Defined Extensions:");
                foreach (var def in evt.ExtensionDefinitions)
                {
                    extensionInfo.AppendLine($"• {def.Name}");
                    foreach (var attr in def.ExtensionAttributeDefinitions)
                    {
                        var typeInfo = (bool)attr.IsReference ? "Reference" : attr.Type;
                        extensionInfo.AppendLine($"  - {attr.Name} ({typeInfo})");
                    }
                }
            }

            // Extension Values
            if (evt.ExtensionValues.Any())
            {
                extensionInfo.AppendLine("\nApplied Extensions:");
                foreach (var val in evt.ExtensionValues)
                {
                    var value = val.Value ?? val.ValueRef;
                    var source = val.Value != null ? "literal" : "reference";
                    extensionInfo.AppendLine($"• {val.ExtensionAttributeDefinition?.Name}: [{source}] {value}");
                }
            }

            if (extensionInfo.Length > 0)
            {
                AddDetailRow(grid, "Extensions:", extensionInfo.ToString(), ref rowIndex);
            }

            return new ScrollViewer
            {
                Content = grid,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(10),
                MaxHeight = 600
            };
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

        private string GetEventDefinitionType(EventDefinition definition)
        {
            return definition switch
            {
                MessageEventDefinition => "Message",
                TimerEventDefinition => "Timer",
                ErrorEventDefinition => "Error",
                EscalationEventDefinition => "Escalation",
                ConditionalEventDefinition => "Condition",
                SignalEventDefinition => "Signal",
                _ => definition.GetType().Name.Replace("EventDefinition", "")
            };
        }

        private string FormatConnections(IEnumerable<SequenceFlow> flows)
        {
            return flows.Any()
                ? string.Join("\n", flows.Select(f => $"• {f.Id}"))
                : null;
        }

        private string FormatLanes(IEnumerable<Lane> lanes)
        {
            return lanes.Any()
                ? string.Join("\n", lanes.Select(l => $"• {l.Name ?? l.Id}"))
                : null;
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
    }
}