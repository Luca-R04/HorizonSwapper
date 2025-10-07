using System.IO;
using System.Text.Json;

namespace HorizonSwapper.Services
{
    public class ConfigService
    {
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public AppConfig LoadConfig()
        {
            if (!File.Exists(_configPath))
                return new AppConfig();

            string json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }

        public void SaveConfig(AppConfig config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
    }

    public class AppConfig
    {
        public string GameDirectory { get; set; }
    }
}
