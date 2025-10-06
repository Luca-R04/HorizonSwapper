using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;

namespace HorizonSwapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                var folderDialog = new OpenFolderDialog
                {
                    Title = "Select a folder",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (folderDialog.ShowDialog() == true)
                {
                    viewModel.SelectedFolderPath = folderDialog.FolderName;
                }
            }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedFolderPath = string.Empty;

                // Notify property changes
                OnPropertyChanged(nameof(viewModel.IsPathSelected));
                OnPropertyChanged(nameof(viewModel.IsNoPathSelected));
            }
        }
        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Check if a character is selected
                var c = viewModel.SelectedCharacter;
                if (c == null)
                {
                    MessageBox.Show("No character selected.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Check if a folder path is selected
                if (string.IsNullOrEmpty(viewModel.SelectedFolderPath))
                {
                    MessageBox.Show("No game directory selected.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Show character details
                MessageBox.Show(
                    $"Name: {c.Name}\nOriginalRootUUID: {c.OriginalRootUUID}\nVariantUUID: {c.VariantUUID}\nSelected Folder: {viewModel.SelectedFolderPath}",
                    "Selected Character",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedCharacter = null;
                MessageBox.Show("Selection has been reset.", "Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}