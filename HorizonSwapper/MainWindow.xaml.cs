using HorizonSwapper.Services;
using HorizonSwapper.ViewModels;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
                    string selectedPath = folderDialog.FolderName;
                    string exePath = System.IO.Path.Combine(selectedPath, "HorizonForbiddenWest.exe");

                    // ✅ Check if the .exe exists
                    if (!System.IO.File.Exists(exePath))
                    {
                        MessageBox.Show(
                            "The selected folder does not contain 'HorizonForbiddenWest.exe'.\n\nPlease select the correct game installation directory.",
                            "Invalid Directory",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return; // stop here
                    }

                    // ✅ Passed the check — set the folder path
                    viewModel.SelectedFolderPath = selectedPath;
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
                var character = viewModel.SelectedCharacter;

                // ✅ Character validation
                if (character == null)
                {
                    MessageBox.Show("No character selected.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ✅ Directory validation
                if (string.IsNullOrEmpty(viewModel.SelectedFolderPath))
                {
                    MessageBox.Show("No game directory selected.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string exePath = Path.Combine(viewModel.SelectedFolderPath, "HorizonForbiddenWest.exe");
                if (!File.Exists(exePath))
                {
                    MessageBox.Show(
                        "The selected folder does not contain HorizonForbiddenWest.exe.\nPlease select a valid game folder.",
                        "Invalid Directory",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                // ✅ Write the .ini file using the service
                CharacterOverrideService.WriteCharacterOverride(
                    viewModel.SelectedFolderPath,
                    character.RootUUID,
                    character.VariantUUID
                );

                // ✅ Confirmation message
                MessageBox.Show(
                    $"Character override saved successfully!\n\nName: {character.Name}\nRootUUID: {character.RootUUID}\nVariantUUID: {character.VariantUUID}",
                    "Character Activated",
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

                // 🧹 Remove the HorizonSwapper.ini file if it exists
                HorizonSwapper.Services.CharacterOverrideService.RemoveCharacterOverrideFile(viewModel.SelectedFolderPath);

                MessageBox.Show("Selection has been reset.", "Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (string.IsNullOrEmpty(viewModel.SelectedFolderPath))
                {
                    MessageBox.Show("No game directory selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string exePath = Path.Combine(viewModel.SelectedFolderPath, "HorizonForbiddenWest.exe");

                if (!File.Exists(exePath))
                {
                    MessageBox.Show("Game executable not found in the selected directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = viewModel.SkipLauncher ? "-nolauncher" : "",
                        WorkingDirectory = viewModel.SelectedFolderPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to launch game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SearchText = string.Empty;
            }
        }

    }
}