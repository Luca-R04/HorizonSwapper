using System.IO;
using System.Text.Json;

namespace HorizonSwapper.Services
{
    public class CharacterService
    {
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Characters.json");

        public List<Character> LoadCharacters()
        {
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

            return characters;
        }

        public IEnumerable<Character> FilterCharactersWithVariant()
        {
            List<Character> characters = this.LoadCharacters();

            return characters.Where(c => c.VariantUUID != null);
        }
    }
}
