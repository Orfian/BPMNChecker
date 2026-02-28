using System.Windows;
using BPMNModel;
using BPMNModel.Model;
using BPMNVisualizer.Services;
using BPMNVisualizer.Simulation;
using Serilog;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Utility;

public class SharedVariables
{
    private static SharedVariables? _instance;
    private static readonly object Lock = new object();

    public ILogger Logger { get; }
    public ModelRoot Model { get; }
    public Dictionary<string, Rect> ObjectBounds { get; }
    public Dictionary<string, IEnumerable<Point>> Paths { get; }
    public Dictionary<string, BPMNShape> Shapes { get; }
    public List<BPMNToken> Tokens { get; }
    public Dictionary<string, SubProcessWindow> SubProcessWindows { get; } = new();

    private SharedVariables(string filePath)
    {
        Logger = LoggerFactory.Create();
        
        var model = new ModelLoader(Logger).LoadModel(filePath);
        
        if (model == null)
        {
            MessageBox.Show("Failed to load BPMN model. Check logs for details.");
            throw new Exception("Failed to load BPMN model. Check logs for details.");
        }
        
        Model = model;
        
        ObjectBounds = new Dictionary<string, Rect>();
        Paths = new Dictionary<string, IEnumerable<Point>>();
        
        Shapes = (Model.AllObjectsWithIds?.Values ?? Enumerable.Empty<object>())
            .OfType<BPMNShape>()
            .Where(s => s.BpmnElement != null)
            .ToDictionary(s => s.BpmnElement!.Id, s => s);    
        
        Tokens = new List<BPMNToken>();
    }

    public static void Initialize(string filePath)
    {
        if (_instance == null)
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new SharedVariables(filePath);
                }
            }
        }
    }

    public static SharedVariables Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("SharedVariables is not initialized. Call SharedVariables.Initialize(filePath) first.");
            }
            return _instance;
        }
    }
}