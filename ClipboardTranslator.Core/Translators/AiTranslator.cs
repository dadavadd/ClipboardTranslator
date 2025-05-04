using ClipboardTranslator.Core.Models;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Translators.Models.AiRequest;
using ClipboardTranslator.Core.Interfaces;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;
using Serilog;

namespace ClipboardTranslator.Core.Translators;

public class AiTranslator(TranslatorConfig config,
                          CancellationToken token = default) : ITranslator
{
    private readonly HttpClient _httpClient = new (new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 20,
        PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),

        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
        KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
    })
    {
        DefaultRequestVersion = new(2, 0)
    };

    private readonly string _translatorEndPoint =
        $"https://generativelanguage.googleapis.com/v1beta/models/"
        + $"{config.GeminiOptions.ModelId}:generateContent"
        + $"?key={config.GeminiOptions.ApiKey}";

    public async Task<string?> TranslateAsync(string text)
    {
        token.ThrowIfCancellationRequested();

        var requestBody = CreateRequestBody(text);
        var translatorResponse = await SendRequestAsync(requestBody, token);
        string? responseStr = await CheckResponseAsync(translatorResponse, token);

        if (responseStr == null)
            throw new InvalidOperationException("Пустой ответ от API перевода.");

        var response = JsonSerializer.Deserialize(responseStr, SerializationConfig.Default.Response)
                      ?? throw new InvalidOperationException("Не удалось десериализовать ответ от API перевода.");

        var result = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(result))
            throw new InvalidOperationException($"Ответ от API перевода не содержит переведённого текста. Ошибка от API: {response.Candidates?.FirstOrDefault()?.FinishReason}");

        return result;
    }

    private async Task<HttpResponseMessage> SendRequestAsync(RequestBody? requestBody, CancellationToken token)
    {
        Log.Information("Запрос для перевода отправлен.");

        var stopWatch = Stopwatch.StartNew();

        var translatorResponse = await _httpClient.PostAsJsonAsync(_translatorEndPoint,
                                                                   requestBody,
                                                                   SerializationConfig.Default.RequestBody, token);

        stopWatch.Stop();

        Log.Information("Ответ на запрос для перевода пришёл за {ElapsedMilliseconds} мс.", stopWatch.ElapsedMilliseconds);

        return translatorResponse;
    }

    private async Task<string?> CheckResponseAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(token);
            Log.Warning("Ошибка при запросе к API: {StatusCode}", response.StatusCode);
            Log.Warning("Ответ с ошибкой: {errorResponse}", errorResponse);
            return null;
        }

        return await response.Content.ReadAsStringAsync(token);
    }

    private RequestBody CreateRequestBody(string text)
    {
        string sourceLang = config.LanguagePair.SourceLang;
        string targetLang = config.LanguagePair.TargetLang;
        string instruction = string.Concat(config.GeminiOptions.Instructions, sourceLang, targetLang);

        return new RequestBody(
            new GenerationConfig(0.95, 40, 8192, "text/plain"),
            [new RequestContent("user", [new RequstPart(text)])],
            new RequestContent("user", [new RequstPart(instruction)])
        );
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        Log.Information($"Вызван Dispose у {nameof(AiTranslator)}");
    }
}
