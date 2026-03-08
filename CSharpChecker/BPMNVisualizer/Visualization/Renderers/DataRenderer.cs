using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using DataObject = BPMNModel.Model.DataObject;

namespace BPMNVisualizer.Visualization.Renderers;

public class DataRenderer : IShapeRenderer
{
    private readonly SharedVariables _vars;

    private readonly Canvas _canvas;
    private readonly BrushManager _brushManager;
    private readonly ShapeManager _shapeManager;
    private readonly SvgResourceManager _svgResourceManager;
    
    private BPMNShape _shape;
    
    private readonly Dictionary<string, FrameworkElement> _dataNotes = new();

    public DataRenderer(Canvas canvas, BrushManager brushManager, ShapeManager shapeManager, SvgResourceManager svgResourceManager)
    {
        _vars = SharedVariables.Instance;
        _canvas = canvas;
        _brushManager = brushManager;
        _shapeManager = shapeManager;
        _svgResourceManager = svgResourceManager;
    }

    public void RenderShape(BPMNShape shape)
    {
        _shape = shape;
        var element = shape.BpmnElement;
        var bounds = new Rect(shape?.Bounds?.X ?? 0, shape?.Bounds?.Y ?? 0, shape?.Bounds?.Width ?? 0, shape?.Bounds?.Height ?? 0);
        
        if (element is not DataInput and not DataOutput and not DataStoreReference and not DataObjectReference)
        {
            _vars.Logger.Warning("DataRenderer received unsupported element type: {ElementType}", element.GetType());
            return;
        }
        _vars.ObjectBounds[element.Id] = bounds;
        var uiElement = DrawElement(element, bounds);
        uiElement.MouseDown += (s, e) => ShowDataDetails(element);
        Canvas.SetLeft(uiElement, bounds.Left);
        Canvas.SetTop(uiElement, bounds.Top);
        _canvas.Children.Add(uiElement);
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
            _vars.Logger.Warning("No icon found for data element type: {DataElementType}", dataElement);
            return null;
        }
        return _shapeManager.WrapInContainer(icon, bounds);
    }

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
                Text = noteText, FontSize = 10, FontStyle = FontStyles.Italic, FontWeight = FontWeights.Normal,
                Foreground = Brushes.DarkSlateGray, TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.CharacterEllipsis
            }
        };
        border.LayoutUpdated += (s, e) => Canvas.SetTop(border, bounds.Top - border.ActualHeight - 5);
        Canvas.SetLeft(border, bounds.Left + 5);
        Canvas.SetTop(border, bounds.Top);
        border.Measure(new Size(bounds.Width, double.PositiveInfinity));
        border.Arrange(new Rect(border.DesiredSize));
        return border;
    }
    
    private void ShowDataDetails(BaseElement element)
    {
        var detailWindow = new DetailWindow
        {
            Owner = Application.Current.MainWindow,
            Title = "Data Details"
        };
        
        detailWindow.SetNote("Data Note:", ElementNotes.GetNote(element.Id) ?? "", (text) => 
        {
            ElementNotes.SetNote(element.Id, text);
            RefreshDataVisual(element);
        });

        // --- Standard Properties ---
        detailWindow.AddStandardProperty("ID", element.Id ?? "null");
        detailWindow.AddStandardProperty("Type", element.GetType().Name);
        
        if (element is DataObject dataObject) 
            detailWindow.AddStandardProperty("Collection", dataObject.IsCollection.ToString());
        if (element is DataInput input) 
             detailWindow.AddStandardProperty("Collection", input.IsCollection.ToString());
        if (element is DataOutput output) 
             detailWindow.AddStandardProperty("Collection", output.IsCollection.ToString());
        
        detailWindow.AddCamundaProperty("Ext Definitions", element.ExtensionDefinitions.Count > 0 ? "Yes" : "");

        detailWindow.Show();
        detailWindow.Activate();
    }


    private void RefreshDataVisual(BaseElement dataElement)
    {
        RefreshDataNote(dataElement);
        if (_dataNotes.TryGetValue(dataElement.Id, out var note)) Panel.SetZIndex(note, int.MaxValue);
    }
    
    private void RefreshDataNote(BaseElement dataElement)
    {
        if (_dataNotes.TryGetValue(dataElement.Id, out var existingNote)) _canvas.Children.Remove(existingNote);
        var note = DrawNote(dataElement, _vars.ObjectBounds[dataElement.Id]);
        if (note != null) {
            note.Tag = $"{dataElement.Id}_note";
            _dataNotes[dataElement.Id] = note;
            Canvas.SetLeft(note, _vars.ObjectBounds[dataElement.Id].Left + 5);
            Canvas.SetTop(note, _vars.ObjectBounds[dataElement.Id].Top - 20);
            _canvas.Children.Add(note);
        }
    }
}
