using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BPMNModel;
using BPMNVisualizer.Services;
using BPMNVisualizer.Simulation;
using BPMNVisualizer.Visualization;
using Microsoft.Win32;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Point = System.Windows.Point;

namespace BPMNVisualizer
{
    public partial class MainWindow : Window
    {
        private readonly ModelLoader _modelLoader;
        private readonly Visualizer _visualizer;
        private readonly ModelRoot? _model;
        private readonly ILogger _logger;
        private readonly Dictionary<string, Rect> _objectBounds = new();
        private readonly Dictionary<string, IEnumerable<Point>> _paths = new();
        
        private TokenManager tokenManager;
        private Simulator simulator;

        public MainWindow()
        {
            //var filePath = @"diagrams/CamundaModeler_almost_all_set.bpmn";
            var filePath = @"diagrams/BookHolidaySagaPatternV2.bpmn";
            //var filePath = @"diagrams/all_icons.bpmn";
            //var filePath = @"diagrams/Multi-instanceMessagingBetweenProcesses-Doctor.bpmn";
            //var filePath = @"diagrams/test.bpmn";
            
            InitializeComponent();
            _logger = LoggerFactory.Create();

            _modelLoader = new ModelLoader(_logger);
            _model = _modelLoader.LoadModel(filePath);
            
            if (_model == null)
            {
                MessageBox.Show("Failed to load BPMN model. Check logs for details.");
                return;
            }
            
            _visualizer = new Visualizer(_logger, BPMNCanvas, _objectBounds, _paths, _model);
            _visualizer.Visualize(_model);
        }
        
        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            tokenManager = new TokenManager(_model, BPMNCanvas);
            simulator = new Simulator(_logger, tokenManager, _model, _objectBounds, _paths);
            
            SimButt.Content = "Next Step";
            SimButt.ToolTip = "Make the next simulation step";
            SimButt.Click -= Simulate_Click;
            SimButt.Click += NextStep_Click;
            
            ResetSimButt.Visibility = Visibility.Visible;
            
            simulator.FirstStep();
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            simulator.NextStep();
        }
        
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            tokenManager.ClearAllTokens();
            simulator.ClearAllActions();
            simulator.ClearPendingChoices();
            
            SimButt.Content = "Simulate";
            SimButt.ToolTip = "Start simulation";
            SimButt.Click -= NextStep_Click;
            SimButt.Click += Simulate_Click;

            ResetSimButt.Visibility = Visibility.Collapsed;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save BPMN Diagram",
                FileName = "bpmn_diagram.png",
                DefaultExt = ".png"
            };

            if (saveDialog.ShowDialog() == true)
            {
                ExportToImage(saveDialog.FileName);

                MessageBox.Show($"Diagram exported successfully to:\n{saveDialog.FileName}",
                    "Export Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        public void ExportToImage(string filePath)
        {
            try
            {
                var contentBounds = VisualTreeHelper.GetDescendantBounds(BPMNCanvas);
                if (contentBounds.IsEmpty || contentBounds.Width < 1 || contentBounds.Height < 1)
                {
                    _logger.Warning("No diagram elements found to export");
                    return;
                }

                var renderTarget = new RenderTargetBitmap(
                    (int)Math.Ceiling(contentBounds.Width),
                    (int)Math.Ceiling(contentBounds.Height),
                    96, 96,
                    PixelFormats.Pbgra32
                );

                var visual = new DrawingVisual();
                using (var dc = visual.RenderOpen())
                {
                    dc.PushTransform(new TranslateTransform(-contentBounds.X, -contentBounds.Y));

                    var vb = new VisualBrush(BPMNCanvas);
                    dc.DrawRectangle(vb, null, new Rect(contentBounds.TopLeft, contentBounds.Size));
                }

                renderTarget.Render(visual);

                using var stream = File.Create(filePath);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(stream);

                _logger.Information($"Exported diagram content to: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Export failed: {ex.Message}");
                MessageBox.Show("Error saving image. See logs for details.");
            }
        }

    }

    public static class LoggerFactory
    {
        public static ILogger Create() => new LoggerConfiguration()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.File(new JsonFormatter(), "important.json", LogEventLevel.Warning)
            .WriteTo.File("all.logs", restrictedToMinimumLevel: LogEventLevel.Verbose,
                rollingInterval: RollingInterval.Day)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }
}