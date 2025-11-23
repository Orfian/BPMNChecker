using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using Serilog;
using DataObject = BPMNModel.Model.DataObject;

namespace BPMNVisualizer.Visualization.Renderers;

public class DataRenderer : IShapeRenderer
{
    private readonly ILogger _logger;
    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;
    private readonly Dictionary<string, Rect> _objectBounds;
    private readonly Dictionary<string, FrameworkElement> _dataNotes = new();

    public DataRenderer(ILogger logger, Canvas canvas, BrushManager brushManager, ShapeManager shapeManager, SvgResourceManager svgResourceManager, Dictionary<string, Rect> objectBounds)
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
        if (element is not DataInput and not DataOutput and not DataStoreReference and not DataObject)
        {
            _logger.Warning("DataRenderer received unsupported element type: {ElementType}", element.GetType());
            return;
        }
        
        _objectBounds[element.Id] = bounds;
        
        var shape = DrawElement(element, bounds);
        shape.MouseDown += (s, e) => ShowDataDetails(element);
        
        Canvas.SetLeft(shape, bounds.Left);
        Canvas.SetTop(shape, bounds.Top);
        _canvas.Children.Add(shape);
        /*
        var label = DrawLabel(element, bounds);
        if (label != null)
        {
            Canvas.SetLeft(label, bounds.Left + (bounds.Width - label.Width) / 2);
            Canvas.SetTop(label, bounds.Top + bounds.Height * 1.1);
            _canvas.Children.Add(label);
        }
        */
        var note = DrawNote(element, bounds);
        if (note != null)
        {
            Canvas.SetLeft(note, bounds.Left + 5);
            Canvas.SetTop(note, bounds.Top - note.Height - 5);
            _canvas.Children.Add(note);
        }
    }
    
    private Border? DrawElement(BaseElement dataElement, Rect bounds)
    {
        var icon = _svgResourceManager.GetDataIcon(dataElement);
        if (icon == null)
        {
            _logger.Warning("No icon found for data element type: {DataElementType}", dataElement);
            return null;
        }
        return _shapeManager.WrapInContainer(icon, bounds);
    }
    /*
    private Border? DrawLabel(BaseElement dataLabel, Rect bounds)
    {
        var text = dataLabel.Id;
        if (string.IsNullOrEmpty(text))
        {
            _logger.Warning("No label found for data element type: {DataElementType}", dataLabel);
            return null;
        }
        
        bounds.Width *= 1.5;
        var label = _shapeManager.GetLabel(text, bounds);
        
        return _shapeManager.WrapInContainer(label, bounds);
    }
    */
    private Border DrawNote(BaseElement dataElement, Rect bounds)
    {
        var noteText = ElementNotes.GetNote(dataElement.Id);
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
    
    private void ShowDataDetails(BaseElement element)
    {
        var detailWindow = new Window
        {
            Title = "Data Details",
            Width = 500,
            Height = 350,
            Content = CreateDetailContent(element),
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        detailWindow.Show();
        detailWindow.Activate();
    }
    
    private UIElement CreateDetailContent(BaseElement element)
    {
        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // ========== Note Section ==========
        var notePanel = new StackPanel { Margin = new Thickness(10) };

        var noteTextBox = new TextBox
        {
            Text = ElementNotes.GetNote(element.Id) ?? "",
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
            ElementNotes.SetNote(element.Id, noteTextBox.Text);
            RefreshDataVisual(element);
        };

        notePanel.Children.Add(new TextBlock
        {
            Text = "Data Note:",
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
            Content = CreateDetailsGrid(element),
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

    private Grid CreateDetailsGrid(BaseElement element)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        int row = 0;
        AddDetailRow(grid, "ID:", element.Id ?? "null", ref row);
        AddDetailRow(grid, "Type:", element.GetType().Name, ref row);

        if (element is DataObject dataObject)
            AddDetailRow(grid, "Is Collection:", dataObject.IsCollection.ToString(), ref row);

        if (element is DataInput input)
            AddDetailRow(grid, "Is Collection:", input.IsCollection.ToString(), ref row);

        if (element is DataOutput output)
            AddDetailRow(grid, "Is Collection:", output.IsCollection.ToString(), ref row);

        return grid;
    }

    private void AddDetailRow(Grid grid, string label, string value, ref int row)
    {
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var lbl = new TextBlock
        {
            Text = label,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(5)
        };
        var val = new TextBlock
        {
            Text = value,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(5)
        };

        Grid.SetRow(lbl, row);
        Grid.SetColumn(lbl, 0);
        Grid.SetRow(val, row);
        Grid.SetColumn(val, 1);

        grid.Children.Add(lbl);
        grid.Children.Add(val);

        row++;
    }

    private void RefreshDataVisual(BaseElement dataElement)
    {
        RefreshDataNote(dataElement);
        
        if (_dataNotes.TryGetValue(dataElement.Id, out var note))
        {
            Panel.SetZIndex(note, int.MaxValue);
        }
    }
    
    private void RefreshDataNote(BaseElement dataElement)
    {
        if (_dataNotes.TryGetValue(dataElement.Id, out var existingNote))
        {
            _canvas.Children.Remove(existingNote);
        }

        var note = DrawNote(dataElement, _objectBounds[dataElement.Id]);
        if (note != null)
        {
            note.Tag = $"{dataElement.Id}_note";
            _dataNotes[dataElement.Id] = note;
            
            Canvas.SetLeft(note, _objectBounds[dataElement.Id].Left + 5);
            Canvas.SetTop(note, _objectBounds[dataElement.Id].Top - 20);
            _canvas.Children.Add(note);
        }
    }
}