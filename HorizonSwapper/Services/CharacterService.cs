using System.IO;
using System.Text.Json;

namespace HorizonSwapper.Services
{
    public class CharacterService
    {
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Characters.json");

        private List<Character>? _allCharactersCache;

        /// <summary>
        /// Load all characters from disk (with caching)
        /// </summary>
        public List<Character> LoadCharacters()
        {
            if (_allCharactersCache != null)
                return _allCharactersCache;

            if (!File.Exists(_path))
                throw new FileNotFoundException($"Characters file not found: {_path}");

            string json = File.ReadAllText(_path);

            // Deserialize into a dictionary first
            var dict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);

            var characters = new List<Character>();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    var name = kvp.Key;
                    var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images", "Characters", $"{name}.jpg");

                    // Only add characters that have an image
                    if (File.Exists(imagePath))
                    {
                        var character = new Character
                        {
                            Name = name,
                            VariantUUID = kvp.Value.Length > 0 ? kvp.Value[0] : string.Empty,
                            RootUUID = kvp.Value.Length > 1 ? kvp.Value[1] : string.Empty
                        };

                        characters.Add(character);
                    }
                }
            }

            _allCharactersCache = GetSortedCharacters(characters);
            return _allCharactersCache;
        }

        private List<Character> GetSortedCharacters(List<Character> characters)
        {
            return characters
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .OrderBy(c => c.Name)
                .ToList();
        }

        /// <summary>
        /// Returns all characters with a variant UUID
        /// </summary>
        public IEnumerable<Character> FilterCharactersWithVariant()
        {
            var characters = LoadCharacters();
            return characters.Where(c => !string.IsNullOrWhiteSpace(c.VariantUUID));
        }

        /// <summary>
        /// ✅ Search characters by name (case-insensitive) and filter only those with variants
        /// </summary>
        public IEnumerable<Character> SearchCharacters(string? searchText)
        {
            var characters = FilterCharactersWithVariant();

            if (string.IsNullOrWhiteSpace(searchText))
                return characters;

            return characters.Where(c =>
                c.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
