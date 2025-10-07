using HorizonSwapper.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HorizonSwapper.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CharacterService _characterService;
        private readonly VariantService _variantService;
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

                    _configService.SaveConfig(new AppConfig { GameDirectory = value });
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
            _variantService = new VariantService();
            _configService = new ConfigService();

            // Load from disk
            var config = _configService.LoadConfig();
            SelectedFolderPath = config.GameDirectory;

            var characters = _characterService.LoadCharacters();
            var variants = _variantService.LoadVariants();

            foreach (var c in _characterService.FilterCharactersWithVariant(characters, variants))
                Characters.Add(c);
        }
    }
}
