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
            return JsonSerializer.Deserialize<List<Character>>(json) ?? new List<Character>();
        }

        public IEnumerable<Character> FilterCharactersWithVariant(IEnumerable<Character> characters, Dictionary<string, string> variants)
        {
            foreach (var c in characters)
            {
                if (variants.TryGetValue(c.OriginalRootUUID, out string variant))
                    c.VariantUUID = variant;
            }

            // remove characters without variants
            return characters.Where(c => c.VariantUUID != null);
        }
    }
}
