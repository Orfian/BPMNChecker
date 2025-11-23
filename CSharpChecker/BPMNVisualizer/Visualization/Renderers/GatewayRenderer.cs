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
    public class GatewayRenderer : IShapeRenderer
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly BrushManager _brushManager;
        private readonly ShapeManager _shapeManager;
        private readonly SvgResourceManager _svgResourceManager;
        private readonly Dictionary<string, Rect> _objectBounds = new();
        private readonly Dictionary<string, FrameworkElement> _gatewayNotes = new();

        public GatewayRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager,
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
            if (element is not Gateway gateway) return;
            
            _objectBounds[gateway.Id] = bounds;

            var shape = DrawElement(gateway, bounds);
            shape.MouseDown += (s, e) => ShowEventDetails(gateway);

            Canvas.SetLeft(shape, bounds.Left);
            Canvas.SetTop(shape, bounds.Top);
            _canvas.Children.Add(shape);

            var icon = DrawIcon(gateway, bounds);
            if (icon != null)
            {
                Canvas.SetLeft(icon, bounds.Left + (bounds.Width - icon.Width) / 2);
                Canvas.SetTop(icon, bounds.Top + (bounds.Height - icon.Height) / 2);
                _canvas.Children.Add(icon);
            }

            var label = DrawLabel(gateway, bounds);
            if (label != null)
            {
                Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
                Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
                _canvas.Children.Add(label);
            }
            
            var note = DrawNote(gateway, bounds);
            if (note != null)
            {
                Canvas.SetLeft(note, bounds.Left + 5);
                Canvas.SetTop(note, bounds.Top - note.Height - 5);
                _canvas.Children.Add(note);
            }
        }

        private UIElement DrawElement(Gateway gateway, Rect bounds)
        {
            var shape = _shapeManager.GetDiamond(bounds, _brushManager.GetGatewayBrush(gateway));
            return shape;
        }

        private Border? DrawIcon(Gateway gateway, Rect bounds)
        {
            var icon = _svgResourceManager.GetGatewayIcon(gateway);
            if (icon == null)
            {
                _logger.Warning("No icon found for gateway type: {GatewayType}", gateway.GetType());
                return null;
            }

            return _shapeManager.WrapInContainer(icon, bounds, 0.25);
        }

        private Border? DrawLabel(Gateway gateway, Rect bounds)
        {
            var text = gateway.Name;
            if (string.IsNullOrEmpty(text))
            {
                _logger.Warning("No label found for gateway type: {GatewayType}", gateway.GetType());
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
        private void ShowEventDetails(Gateway gateway)
        {
            var detailWindow = new Window
            {
                Title = "Gateway Details",
                Width = 600,
                Height = 400,
                Content = CreateDetailContent(gateway),
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            detailWindow.Show();
            detailWindow.Activate();
        }
        
        private UIElement CreateDetailContent(Gateway gateway)
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // ========== Note Section ==========
            var notePanel = new StackPanel { Margin = new Thickness(10) };
    
            var noteTextBox = new TextBox
            {
                Text = ElementNotes.GetNote(gateway.Id) ?? "",
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
                ElementNotes.SetNote(gateway.Id, noteTextBox.Text);
                RefreshGatewayVisual(gateway);
            };
    
            notePanel.Children.Add(new TextBlock { 
                Text = "Gateway Note:", 
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
                Content = CreateDetailsGrid(gateway),
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

        private Grid CreateDetailsGrid(Gateway gateway)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int rowIndex = 0;

            // ========== Core Properties ==========
            AddDetailRow(grid, "ID:", gateway.Id ?? "null", ref rowIndex);
            AddDetailRow(grid, "Name:", gateway.Name ?? "null", ref rowIndex);
            AddDetailRow(grid, "Type:", gateway.GetType().Name, ref rowIndex);
            AddDetailRow(grid, "Direction:", gateway.GatewayDirection?.ToString() ?? "null", ref rowIndex);

            // ========== Gateway-Specific Properties ==========
            switch (gateway)
            {
                case ExclusiveGateway exclusive:
                    AddDetailRow(grid, "Default Flow:", exclusive.Default?.Id ?? "None", ref rowIndex);
                    break;

                case InclusiveGateway inclusive:
                    AddDetailRow(grid, "Default Flow:", inclusive.Default?.Id ?? "None", ref rowIndex);
                    break;

                case EventBasedGateway eventGateway:
                    AddDetailRow(grid, "Instantiate:", eventGateway.Instantiate?.ToString() ?? "null", ref rowIndex);
                    AddDetailRow(grid, "Event Type:", eventGateway.EventGatewayType?.ToString() ?? "null", ref rowIndex);
                    break;

                case ComplexGateway complex:
                    AddDetailRow(grid, "Activation Condition:",
                       complex.ActivationCondition != null ? (complex.ActivationCondition is FormalExpression expr?  (expr.Body == null? "null" : expr.Body) : complex.ActivationCondition) :  "Null" ,
                       ref rowIndex);
                    AddDetailRow(grid, "Default Flow:", complex.Default?.Id ?? "None", ref rowIndex);
                    break;

                case ParallelGateway parallel:
                    var syncType = parallel.GatewayDirection switch
                    {
                        GatewayDirection.Converging => "Join (Synchronization)",
                        GatewayDirection.Diverging => "Split (Fork)",
                        _ => "Unknown"
                    };
                    AddDetailRow(grid, "Sync Type:", syncType, ref rowIndex);
                    break;
            }

            // ========== Flow Relationships ==========
            AddDetailRow(grid, "Incoming:", FormatConnections(gateway.Incoming), ref rowIndex);
            AddDetailRow(grid, "Outgoing:", FormatConnections(gateway.Outgoing), ref rowIndex);
            AddDetailRow(grid, "Lanes:", FormatLanes(gateway.Lanes), ref rowIndex);

            // ========== Connection Analysis ==========
            var flowAnalysis = new StringBuilder();
            flowAnalysis.AppendLine($"Split/Join Type: {GetSplitJoinType(gateway)}");

            // Show conditions for outgoing flows
            flowAnalysis.Append(GetFlowConditions(gateway));

            AddDetailRow(grid, "Flow Analysis:", flowAnalysis.ToString(), ref rowIndex);

            // ========== Enhanced Camunda Extensions ==========
            var camundaInfo = new StringBuilder();

            // General Camunda properties
            if (gateway.Camunda_asyncBefore.HasValue)
                camundaInfo.AppendLine($"• Async Before: {gateway.Camunda_asyncBefore}");
            if (gateway.Camunda_asyncAfter.HasValue)
                camundaInfo.AppendLine($"• Async After: {gateway.Camunda_asyncAfter}");
            if (gateway.Camunda_exclusive.HasValue)
                camundaInfo.AppendLine($"• Exclusive: {gateway.Camunda_exclusive}");
            if (!string.IsNullOrEmpty(gateway.Camunda_jobPriority))
                camundaInfo.AppendLine($"• Job Priority: {gateway.Camunda_jobPriority}");

            // Camunda elements
            var camundaElements = gateway.CamundaElements;

            // Input/Output Mappings
            var ioMappings = camundaElements.OfType<CamundaInputOutput>();
            if (ioMappings.Any())
            {
                camundaInfo.AppendLine("\nI/O Mappings:");
                foreach (var io in ioMappings)
                {
                    foreach (var input in io.InputParameters)
                    {
                        camundaInfo.AppendLine($"• Input {input.Name}: {input.Value}");
                    }

                    foreach (var output in io.OutputParameters)
                    {
                        camundaInfo.AppendLine($"• Output {output.Name}: {output.Value}");
                    }
                }
            }

            // Execution Listeners
            var listeners = camundaElements.OfType<CamundaExecutionListener>();
            if (listeners.Any())
            {
                camundaInfo.AppendLine("\nExecution Listeners:");
                foreach (var listener in listeners)
                {
                    camundaInfo.AppendLine($"• {listener.Event}: {listener.Class ?? listener.Expression}");
                }
            }

            // Connectors
            var connectors = camundaElements.OfType<CamundaConnector>();
            if (connectors.Any())
            {
                camundaInfo.AppendLine("\nConnectors:");
                foreach (var conn in connectors)
                {
                    camundaInfo.AppendLine($"• {conn.ConnectorId}:");
                    if (conn.InputOutput != null)
                    {
                        foreach (var param in conn.InputOutput.InputParameters)
                        {
                            camundaInfo.AppendLine($"  - Input {param.Name}: {param.Value}");
                        }

                        foreach (var param in conn.InputOutput.OutputParameters)
                        {
                            camundaInfo.AppendLine($"  - Output {param.Name}: {param.Value}");
                        }
                    }
                }
            }

            if (camundaInfo.Length > 0)
            {
                AddDetailRow(grid, "Camunda Properties:", camundaInfo.ToString(), ref rowIndex);
            }

            // ========== Enhanced Extensions ==========
            var extensionInfo = new StringBuilder();

            // Extension Definitions
            if (gateway.ExtensionDefinitions.Any())
            {
                extensionInfo.AppendLine("Defined Extensions:");
                foreach (var def in gateway.ExtensionDefinitions)
                {
                    extensionInfo.AppendLine($"• {def.Name}");
                    foreach (var attr in def.ExtensionAttributeDefinitions)
                    {
                        extensionInfo.AppendLine($"  - {attr.Name} ({attr.Type})");
                    }
                }
            }

            // Extension Values
            if (gateway.ExtensionValues.Any())
            {
                extensionInfo.AppendLine("\nApplied Extensions:");
                foreach (var val in gateway.ExtensionValues)
                {
                    var value = val.Value ?? val.ValueRef;
                    extensionInfo.AppendLine($"• {val.ExtensionAttributeDefinition?.Name}: {value}");
                }
            }

            if (extensionInfo.Length > 0)
            {
                AddDetailRow(grid, "Extensions:", extensionInfo.ToString(), ref rowIndex);
            }

            // ========== Documentation ==========
            if (gateway.Documentation.Any())
            {
                AddDetailRow(grid, "Docs:", FormatDocumentation(gateway.Documentation), ref rowIndex);
            }

            return grid;
        }

        // ========== Helper Methods ==========
        private string GetSplitJoinType(Gateway gateway)
        {
            return gateway.GatewayDirection switch
            {
                GatewayDirection.Diverging => "Split Gateway",
                GatewayDirection.Converging => "Join Gateway",
                GatewayDirection.Mixed => "Mixed Split/Join",
                _ => "Unspecified Direction"
            };
        }

        private string GetFlowConditions(Gateway gateway)
        {
            var sb = new StringBuilder();

            foreach (var flow in gateway.Outgoing)
            {
                sb.Append($"• {flow.Id}: ");

                if (flow == GetDefaultFlow(gateway))
                    sb.Append("[DEFAULT] ");

                if (flow.ConditionExpression is FormalExpression expr)
                    sb.AppendLine(expr.Body.Value);
                else if (gateway is InclusiveGateway)
                    sb.AppendLine("Inclusive Condition");
                else
                    sb.AppendLine("No Condition");
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
        
        private void RefreshGatewayVisual(Gateway gateway)
        {
            RefreshGatewayNote(gateway);
        
            if (_gatewayNotes.TryGetValue(gateway.Id, out var note))
            {
                Panel.SetZIndex(note, int.MaxValue);
            }
        }
    
        private void RefreshGatewayNote(Gateway gateway)
        {
            if (_gatewayNotes.TryGetValue(gateway.Id, out var existingNote))
            {
                _canvas.Children.Remove(existingNote);
            }

            var note = DrawNote(gateway, _objectBounds[gateway.Id]);
            if (note != null)
            {
                note.Tag = $"{gateway.Id}_note";
                _gatewayNotes[gateway.Id] = note;
            
                Canvas.SetLeft(note, _objectBounds[gateway.Id].Left + 5);
                Canvas.SetTop(note, _objectBounds[gateway.Id].Top - 20);
                _canvas.Children.Add(note);
            }
        }
    }
}