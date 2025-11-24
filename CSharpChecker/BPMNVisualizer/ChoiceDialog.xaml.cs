using System.Collections.Generic;
        using System.Linq;
        using System.Windows;
        using System.Windows.Controls;
        
        namespace BPMNVisualizer
        {
            public partial class ChoiceDialog : Window
            {
                public List<string> SelectedOptions { get; private set; } = new();
                public string DialogTitle { get; }
                public string Message { get; }
                public SelectionMode SelectionMode { get; }
        
                private readonly string _defaultOption;
        
                public ChoiceDialog(string title, string message, List<string> options, bool multiSelect, string defaultOption = "")
                {
                    InitializeComponent();
                    DialogTitle = title;
                    Message = message;
                    SelectionMode = multiSelect ? SelectionMode.Multiple : SelectionMode.Single;
                    DataContext = this;
                    
                    if (!string.IsNullOrEmpty(defaultOption))
                    {
                        _defaultOption = defaultOption + " (default)";
                        options.Insert(0, _defaultOption);
                    }
                    OptionsList.ItemsSource = options;
                }
                
                private void OptionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    if (string.IsNullOrEmpty(_defaultOption)) return;
                    if (!(sender is ListBox listBox)) return;

                    bool def = e.AddedItems.Cast<object>().OfType<string>().Any(s => s == _defaultOption);
                    if (def)
                    {
                        var nonDefaults = listBox.SelectedItems
                            .Cast<object>()
                            .OfType<string>()
                            .Where(s => s != _defaultOption)
                            .ToList();

                        foreach (var item in nonDefaults)
                            listBox.SelectedItems.Remove(item);

                        return;
                    }

                    bool nonDef = e.AddedItems.Cast<object>().OfType<string>().Any(s => s != _defaultOption);
                    if (nonDef && listBox.SelectedItems.Cast<object>().OfType<string>().Any(s => s == _defaultOption))
                    {
                        listBox.SelectedItems.Remove(_defaultOption);
                    }
                }
                
                private void Ok_Click(object sender, RoutedEventArgs e)
                {
                    SelectedOptions =
                        OptionsList.SelectedItems.Cast<string>()
                            .Select(option => option == _defaultOption ? option.Replace(" (default)", "") : option)
                            .ToList();
                    DialogResult = true;
                }
        
                private void Cancel_Click(object sender, RoutedEventArgs e)
                {
                    DialogResult = false;
                }
            }
        }