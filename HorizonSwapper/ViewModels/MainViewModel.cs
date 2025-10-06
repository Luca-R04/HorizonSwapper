using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Character> Characters { get; set; } = new ObservableCollection<Character>();

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

    private Dictionary<string, string> _variantByRoot;

    private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

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

                SaveConfig();
            }
        }
    }

    public bool IsNoPathSelected => string.IsNullOrEmpty(SelectedFolderPath);
    public bool IsPathSelected => !IsNoPathSelected;

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public MainViewModel()
    {
        LoadConfig();
        LoadCharacters();
        LoadVariants();
        FilterCharactersWithVariant();
    }

    // Load your predefined characters
    private void LoadCharacters()
    {
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        string jsonPath = Path.Combine(exeDir, "Data", "Characters.json");

        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Characters file not found: {jsonPath}");

        string json = File.ReadAllText(jsonPath);

        var characters = JsonSerializer.Deserialize<List<Character>>(json);

        Characters.Clear();
        foreach (var c in characters)
        {
            Characters.Add(c);
        }
    }


    // Load the JSON mapping and build dictionary
    private void LoadVariants()
    {
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        string jsonPath = Path.Combine(exeDir, "Data", "Variant_IDs.json");
        string json = File.ReadAllText(jsonPath);

        var entries = JsonSerializer.Deserialize<List<VariantEntry>>(json);

        // Keep only the first occurrence per RootUUID
        _variantByRoot = entries
            .GroupBy(e => e.RootUUID)
            .ToDictionary(g => g.Key, g => g.First().VariantUUID);
    }


    // Filter characters that have a matching RootUUID
    private void FilterCharactersWithVariant()
    {
        foreach (var character in Characters)
        {
            if (_variantByRoot.TryGetValue(character.OriginalRootUUID, out string variant))
            {
                character.VariantUUID = variant;
            }
        }

        // Optionally remove characters without a variant:
        var toRemove = Characters.Where(c => c.VariantUUID == null).ToList();
        foreach (var c in toRemove)
            Characters.Remove(c);
    }

    private void SaveConfig()
    {
        var config = new { GameDirectory = SelectedFolderPath };
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }

    private void LoadConfig()
    {
        if (File.Exists(_configPath))
        {
            try
            {
                var json = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (config != null && config.TryGetValue("GameDirectory", out var path))
                {
                    SelectedFolderPath = path;
                }
            }
            catch
            {
                // ignore errors
            }
        }
    }

}
