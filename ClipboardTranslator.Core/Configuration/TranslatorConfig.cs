using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using ClipboardTranslator.Core.Translators;

namespace ClipboardTranslator.Core.Configuration;

public partial class TranslatorConfig
{
    public required string Proxy { get; set; } 
    public required string TranslationMode { get; set; }
    public required GeminiOptions GeminiOptions { get; set; }
    public required LanguagePair LanguagePair { get; set; }

    public static TranslatorConfig Load()
    {
        string jsonConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

        if (!File.Exists(jsonConfigPath))
            throw new FileNotFoundException("Конфиг не найден.", jsonConfigPath);

        string jsonConfig = File.ReadAllText(jsonConfigPath);

        var config = JsonSerializer.Deserialize(jsonConfig, SerializationConfig.Default.TranslatorConfig)
            ?? throw new InvalidOperationException("Ошибка при десериализации конфига.");

        ProxyManager.SetProxyIfNeeded(config);

        return config;
    }

}
