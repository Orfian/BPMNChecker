using System.Windows;
using System.Windows.Input;
using BPMNVisualizer.Simulation;
using BPMNVisualizer.Simulation.History;

namespace BPMNVisualizer
{
    public partial class HistoryWindow : Window
    {
        private readonly Simulator _simulator;

        public HistoryWindow(Simulator simulator)
        {
            InitializeComponent();
            _simulator = simulator;
            HistoryList.ItemsSource = _simulator.History;
        }

        private void HistoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HistoryList.SelectedItem is SimulationState state)
            {
                _simulator.LoadState(state.StepIndex);
            }
        }

        private void LoadState_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryList.SelectedItem is SimulationState state)
            {
                _simulator.LoadState(state.StepIndex);
            }
            else
            {
                MessageBox.Show("Please select a simulation step to load.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
