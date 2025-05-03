using System.Windows;
using BPMNVisualizer.Services;
using BPMNVisualizer.Visualization;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace BPMNVisualizer
{
    public partial class MainWindow : Window
    {
        private readonly ModelLoader _modelLoader;
        private readonly Visualizer _visualizer;
        private readonly ILogger _logger;

        public MainWindow()
        {
            InitializeComponent();
            _logger = LoggerFactory.Create();
            
            _modelLoader = new ModelLoader(_logger);
            _visualizer = new Visualizer(_logger, BPMNCanvas);

            LoadAndVisualizeBPMN(@"diagrams/test.bpmn");
        }

        private void LoadAndVisualizeBPMN(string filePath)
        {
            var model = _modelLoader.LoadModel(filePath);
            _visualizer.Visualize(model);
        }
    }

    public static class LoggerFactory
    {
        public static ILogger Create() => new LoggerConfiguration()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.File(new JsonFormatter(), "important.json", LogEventLevel.Warning)
            .WriteTo.File("all.logs", restrictedToMinimumLevel: LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }
}