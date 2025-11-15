using HorizonSwapper.Domain;
using HorizonSwapper.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HorizonSwapper.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CharacterService _characterService;
        private readonly ConfigService _configService;

        public ObservableCollection<Character> Characters { get; } = new();

        private Character _selectedCharacter;
        public Character SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (_selectedCharacter != value)
                {
                    _selectedCharacter = value;
                    OnPropertyChanged(nameof(SelectedCharacter));
                }
            }
        }

        private string _selectedFolderPath;
        private bool _skipLauncher;

        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set
            {
                if (_selectedFolderPath != value)
                {
                    _selectedFolderPath = value;
                    OnPropertyChanged(nameof(SelectedFolderPath));
                    OnPropertyChanged(nameof(IsNoPathSelected));
                    OnPropertyChanged(nameof(IsPathSelected));

                    SaveConfig();
                }
            }
        }

        public bool SkipLauncher
        {
            get => _skipLauncher;
            set
            {
                if (_skipLauncher != value)
                {
                    _skipLauncher = value;
                    OnPropertyChanged(nameof(SkipLauncher));

                    SaveConfig();
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilter();
                }
            }
        }


        public bool IsNoPathSelected => string.IsNullOrEmpty(SelectedFolderPath);
        public bool IsPathSelected => !IsNoPathSelected;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MainViewModel()
        {
            _characterService = new CharacterService();
            _configService = new ConfigService();

            // Load from disk
            var config = _configService.LoadConfig();
            SelectedFolderPath = config.GameDirectory;
            SkipLauncher = config.SkipLauncher;


            foreach (var c in _characterService.FilterCharactersWithVariant())
                Characters.Add(c);
        }

        private void SaveConfig()
        {
            _configService.SaveConfig(new AppConfig
            {
                GameDirectory = SelectedFolderPath,
                SkipLauncher = SkipLauncher
            });
        }

        private void ApplyFilter()
        {
            Characters.Clear();
            var filtered = _characterService.SearchCharacters(SearchText);

            foreach (var c in filtered)
                Characters.Add(c);
        }

    }
}
