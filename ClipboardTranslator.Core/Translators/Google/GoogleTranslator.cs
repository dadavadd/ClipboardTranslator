using Serilog;
using System.Text.Json;
using ClipboardTranslator.Core.Configuration;
using System.Diagnostics;
using ClipboardTranslator.Core.Interfaces;
using ClipboardTranslator.Core.Exceptions;

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
        try
        {
            return await _httpClient.GetAsync(requestBody, token);
        }
        catch (HttpRequestException ex)
        {
            throw new TranslatorException("Проблема с соединением к серверу перевода", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new TranslatorException("Сервер перевода не ответил вовремя", ex);
        }
    }

    private async Task<string> CheckResponseAsync(HttpResponseMessage response, CancellationToken token)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(token);
    }

    private string? GetResponseText(JsonElement element) =>
        string.Concat(element[0].EnumerateArray()
            .Where(s => s.ValueKind == JsonValueKind.Array && s.GetArrayLength() > 1)
            .Select(s => s[0].GetString()));
}
