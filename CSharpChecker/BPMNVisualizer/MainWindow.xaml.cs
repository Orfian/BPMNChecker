using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BPMNVisualizer.Services;
using BPMNVisualizer.Visualization;
using Microsoft.Win32;
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

            LoadAndVisualizeBPMN(@"diagrams/CamundaModeler_almost_all_set.bpmn");
        }

        private void LoadAndVisualizeBPMN(string filePath)
        {
            var model = _modelLoader.LoadModel(filePath);
            _visualizer.Visualize(model);
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
                // Temporarily reset scroll position
                var originalVerticalOffset = MainScrollViewer.ContentVerticalOffset;
                var originalHorizontalOffset = MainScrollViewer.ContentHorizontalOffset;
                MainScrollViewer.ScrollToTop();
                MainScrollViewer.ScrollToLeftEnd();
                
                // Force UI update
                BPMNCanvas.UpdateLayout();

                // Get full content dimensions
                double contentWidth = MainScrollViewer.ExtentWidth;
                double contentHeight = MainScrollViewer.ExtentHeight;
                
                // Create render target
                var renderTarget = new RenderTargetBitmap(
                    (int)Math.Ceiling(contentWidth),
                    (int)Math.Ceiling(contentHeight),
                    96, 96, PixelFormats.Pbgra32);

                // Render canvas
                renderTarget.Render(BPMNCanvas);

                // Restore original scroll position
                MainScrollViewer.ScrollToVerticalOffset(originalVerticalOffset);
                MainScrollViewer.ScrollToHorizontalOffset(originalHorizontalOffset);

                // Save to file
                using var stream = File.Create(filePath);
                new PngBitmapEncoder { Frames = { BitmapFrame.Create(renderTarget) } }.Save(stream);

                _logger.Information($"Exported full diagram to: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Export failed: {ex.Message}");
                MessageBox.Show("Error saving image. Check path permissions.");
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