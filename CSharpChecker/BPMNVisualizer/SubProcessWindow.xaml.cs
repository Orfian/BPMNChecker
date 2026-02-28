using System.ComponentModel;
using System.Windows;
using BPMNModel.Model;
using BPMNVisualizer.Simulation;
using BPMNVisualizer.Utility;
using BPMNVisualizer.Visualization;

namespace BPMNVisualizer
{
    public partial class SubProcessWindow : Window
    {
        private TokenManager _tokenManager;

        public SubProcessWindow()
        {
            InitializeComponent();
        }

        private bool _isForceClosing;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isForceClosing)
            {
                base.OnClosing(e);
                return;
            }
            
            // Hide instead of close to preserve token visuals on the canvas
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// Actually closes and destroys the window. Call this when the simulation is reset.
        /// </summary>
        public void ForceClose()
        {
            _isForceClosing = true;
            Close();
        }

        public void RenderSubProcess(BPMNDiagram diagram)
        {
            var visualizer = new Visualizer(SubProcessCanvas);
            visualizer.VisualizeDiagram(diagram);
        }

        public void StartSimulation(SubProcess subProcess, BPMNToken parentToken)
        {
            _tokenManager = new TokenManager(SubProcessCanvas);

            foreach (var startEvent in subProcess.FlowElements.OfType<StartEvent>())
            {
                if (SharedVariables.Instance.ObjectBounds.TryGetValue(startEvent.Id!, out var bounds))
                {
                    var token = _tokenManager.AddToken(startEvent, bounds);
                    token.Parent = parentToken;
                }
            }
        }
    }
}

