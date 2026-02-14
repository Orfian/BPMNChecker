using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BPMNVisualizer
{
    public partial class DetailWindow : Window
    {
        private Action<string> _onSaveNote;

        public DetailWindow()
        {
            InitializeComponent();
        }

        public void SetNote(string label, string noteContent, Action<string> onSaveNote)
        {
            NoteLabel.Text = label;
            NoteTextBox.Text = noteContent;
            _onSaveNote = onSaveNote;
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            _onSaveNote?.Invoke(NoteTextBox.Text);
        }

        public void AddStandardProperty(string label, string value)
        {
            AddProperty(StandardPanel, label, value);
        }

        public void AddCamundaProperty(string label, string value)
        {
            AddProperty(CamundaPanel, label, value);
        }

        public void SetScript(string scriptContent)
        {
            if (!string.IsNullOrWhiteSpace(scriptContent))
            {
                ScriptTextBox.Text = scriptContent;
                ScriptPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ScriptPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void Clear()
        {
             StandardPanel.Children.Clear();
             CamundaPanel.Children.Clear();
             ScriptPanel.Visibility = Visibility.Collapsed;
        }

        private void AddProperty(StackPanel panel, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            var grid = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var lbl = new TextBlock 
            { 
                Text = $"{label}:", 
                FontWeight = FontWeights.Bold, 
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap
            };
            var val = new TextBlock 
            { 
                Text = value, 
                FontFamily = new FontFamily("Consolas"), 
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top
            };

            Grid.SetColumn(lbl, 0);
            Grid.SetColumn(val, 1);
            grid.Children.Add(lbl);
            grid.Children.Add(val);
            
            panel.Children.Add(grid);
        }
    }
}
