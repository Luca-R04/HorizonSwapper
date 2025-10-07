using System.IO;
using System.Text.Json;

namespace HorizonSwapper.Services
{
    public class VariantService
    {
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Variant_IDs.json");

        public Dictionary<string, string> LoadVariants()
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException($"Variant IDs file not found: {_path}");

            string json = File.ReadAllText(_path);
            var entries = JsonSerializer.Deserialize<List<VariantEntry>>(json) ?? new List<VariantEntry>();

            return entries
                .GroupBy(e => e.RootUUID)
                .ToDictionary(g => g.Key, g => g.First().VariantUUID);
        }
    }
}
