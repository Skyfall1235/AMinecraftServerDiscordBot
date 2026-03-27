using System.Text.Json;

namespace ASimpleMinecraftUpdatesBot.Services
{
    public class JsonService
    {
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.json");
        public BotConfig Config { get; private set; }

        public JsonService()
        {
            if (!Directory.Exists("data")) Directory.CreateDirectory("data");
            Config = Load();
        }

        public BotConfig Load()
        {
            if (!File.Exists(_path)) return new BotConfig();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<BotConfig>(json) ?? new BotConfig();
        }

        public void SaveConfig(BotConfig newConfig)
        {
            Config = newConfig;
            SaveTextToFile();
        }

        public void SaveTextToFile() => File.WriteAllText(_path, JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
    }
}
