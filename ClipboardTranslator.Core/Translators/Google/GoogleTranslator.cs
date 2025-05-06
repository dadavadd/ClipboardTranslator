using Serilog;
using System.Text.Json;
using ClipboardTranslator.Core.Configuration;
using System.Diagnostics;
using ClipboardTranslator.Core.Interfaces;

namespace ClipboardTranslator.Core.Translators.Google;

public class GoogleTranslator(TranslatorConfig config,
                              CancellationToken token = default) : ITranslator
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private string _translationEndPoint = "https://translate.googleapis.com/translate_a/" +
        $"single?client=gtx&sl={config.LanguagePair.SourceLang}" +
        $"&tl={config.LanguagePair.TargetLang}&dt=t&q=";

    public async Task<string?> TranslateAsync(string text)
    {
        token.ThrowIfCancellationRequested();

        string finalUrl = _translationEndPoint + Uri.EscapeDataString(text);

        using var response = await GetRequestAsync(finalUrl, token);
        var responseStr = await CheckResponseAsync(response, token);

        var jsonResponse = JsonSerializer.Deserialize(responseStr, SerializationConfig.Default.JsonElement);

        return GetResponseText(jsonResponse);
    }

    private async Task<HttpResponseMessage> GetRequestAsync(string requestBody, CancellationToken token)
    {
        Log.Information("Запрос для перевода отправлен.");

        var stopWatch = Stopwatch.StartNew();

        var response = await _httpClient.GetAsync(requestBody, token);

        stopWatch.Stop();

        Log.Information("Ответ на запрос для перевода пришёл за {ElapsedMilliseconds} мс.", stopWatch.ElapsedMilliseconds);

        return response;
    }

    private async Task<string> CheckResponseAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(token);
            Log.Warning("Ошибка при запросе к API: {StatusCode}", response.StatusCode);
            Log.Warning("Ответ с ошибкой: {errorResponse}", errorResponse);
            throw new InvalidOperationException("Пустой ответ от API перевода.");
        }

        return await response.Content.ReadAsStringAsync(token);
    }

    private string GetResponseText(JsonElement element)
    {
        string translatedText = string.Empty;

        foreach (var sentence in element[0].EnumerateArray())
        {
            if (sentence.ValueKind == JsonValueKind.Array && sentence.GetArrayLength() > 1)
            {
                translatedText += sentence[0].GetString();
            }
        }

        return translatedText;
    }
}
