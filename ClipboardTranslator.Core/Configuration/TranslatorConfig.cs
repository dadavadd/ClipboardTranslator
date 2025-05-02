using System.Text.Json;
using ClipboardTranslator.Core.Models;
using ClipboardTranslator.Core.Translators.Models;

namespace ClipboardTranslator.Core.Configuration;

public record TranslatorConfig(string TranslationMode,
                               GeminiOptions GeminiOptions,
                               LanguagePair LanguagePair)
{
    public static TranslatorConfig Load()
    {
        string jsonConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

        if (!File.Exists(jsonConfigPath))
            throw new FileNotFoundException("Config file not found.", jsonConfigPath);

        string jsonConfig = File.ReadAllText(jsonConfigPath);

        var config = JsonSerializer.Deserialize(jsonConfig, SerializationConfig.Default.TranslatorConfig);

        if (config == null)
            throw new InvalidOperationException("Failed to deserialize the configuration file.");

        return config;
    }
}
