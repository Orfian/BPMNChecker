using System.Windows;
using BPMNVisualizer.Simulation;

namespace BPMNVisualizer
{
    public partial class QueueDialog
    {
        public ISimulationQueueItem? SelectedItem { get; private set; }
        public bool TriggerWithoutSelection { get; private set; }

        public QueueDialog(string title, string triggerButtonText, IEnumerable<ISimulationQueueItem> items)
        {
            InitializeComponent();
            Title = title;
            TriggerButton.Content = triggerButtonText;
            HeaderBlock.Text = $"Select a {title.ToLower().Replace(" queue", "")} to process:";
            ItemsGrid.ItemsSource = items;
        }

        private void ProcessSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is ISimulationQueueItem item)
            {
                SelectedItem = item;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select an item.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TriggerWithoutSelection_Click(object sender, RoutedEventArgs e)
        {
            TriggerWithoutSelection = true;
            DialogResult = true;
            Close();
        }

        private void ItemsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProcessSelected_Click(sender, e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
