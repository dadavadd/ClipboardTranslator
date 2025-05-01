using System.Text.Json;
using ClipboardTranslator.Models;

namespace ClipboardTranslator.Configuration;

public record Config(GeminiOptions GeminiOptions, LanguagePair LanguagePair)
{
    public static Config Load()
    {
        string jsonConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

        if (!File.Exists(jsonConfigPath))
            throw new FileNotFoundException("Config file not found.", jsonConfigPath);

        string jsonConfig = File.ReadAllText(jsonConfigPath);

        var config = JsonSerializer.Deserialize(jsonConfig, SerializationConfig.Default.Config);

        if (config == null)
            throw new InvalidOperationException("Failed to deserialize the configuration file.");

        return config;
    }
}
