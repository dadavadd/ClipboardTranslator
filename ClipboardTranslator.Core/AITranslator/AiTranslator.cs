using ClipboardTranslator.Core.Models;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.AITranslator.Models.AiRequest;
using ClipboardTranslator.Core.Interfaces;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;
using Serilog;

namespace ClipboardTranslator.Core.AITranslator;

public class AiTranslator(TranslatorConfig config, HttpClient? httpClient = null) : IAiTranslator
{
    private readonly HttpClient _httpClient = httpClient ?? new (new SocketsHttpHandler
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
        try
        {
            var requestBody = CreateRequestBody(text);

            Log.Information("Запрос для перевода отправлен.");

            var stopWatch = Stopwatch.StartNew();

            var translatorResponse = await _httpClient.PostAsJsonAsync(_translatorEndPoint,
                                                                       requestBody,
                                                                       SerializationConfig.Default.RequestBody);

            stopWatch.Stop();

            Log.Information("Ответ на запрос для перевода пришёл за {ElapsedMilliseconds} мс.", stopWatch.ElapsedMilliseconds);

            string? responseStr;
            if (!translatorResponse.IsSuccessStatusCode)
            {
                responseStr = await translatorResponse.Content.ReadAsStringAsync();
                Log.Warning("Ответ от API перевода завершился с ошибкой: {StatusCode}", translatorResponse.StatusCode);
                Log.Warning("Ответ с ошибкой: {responseStr}", responseStr);
                return null;
            }

            responseStr = await translatorResponse.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize(responseStr, SerializationConfig.Default.Response);

            if (response == null)
            {
                Log.Warning("Не удалось десериализовать ответ от API перевода.");
                return null;
            }

            var result = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrWhiteSpace(result))
            {
                Log.Warning("Ответ от API перевода не содержит текста.");
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Произошла ошибка при переводе текста: {Message}", ex.Message);
            return null;
        }
    }

    private RequestBody CreateRequestBody(string text)
    {
        string sourceLang = config.LanguagePair.SourceLang;
        string targetLang = config.LanguagePair.TargetLang;
        string instruction = string.Concat(config.GeminiOptions.Instructions, sourceLang, targetLang);

        return new RequestBody(
            new GenerationConfig(0, 1, 8192, "text/plain"),
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
