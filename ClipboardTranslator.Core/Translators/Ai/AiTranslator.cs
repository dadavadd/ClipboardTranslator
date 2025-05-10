using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Interfaces;
using ClipboardTranslator.Core.Translators.Ai.Models.AiRequest;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;
using Serilog;
using ClipboardTranslator.Core.Exceptions;

namespace ClipboardTranslator.Core.Translators.Ai;

public class AiTranslator(TranslatorConfig config,
                          CancellationToken token = default) : ITranslator
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _translatorEndPoint =
        $"https://generativelanguage.googleapis.com/v1beta/models/"
        + $"{config.GeminiOptions.ModelId}:generateContent"
        + $"?key={config.GeminiOptions.ApiKey}";

    public async Task<string?> TranslateAsync(string text)
    {
        token.ThrowIfCancellationRequested();
        var requestBody = CreateRequestBody(text);
        using var translatorResponse = await SendRequestAsync(requestBody, token);
        string responseStr = await CheckResponseAsync(translatorResponse, token);

        var response = JsonSerializer.Deserialize(responseStr, SerializationConfig.Default.AiResponseBody)
                      ?? throw new TranslatorException("Не удалось десериализовать ответ от API перевода.");

        var result = response.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        return !string.IsNullOrWhiteSpace(result) ? result : throw new TranslatorException($"Ответ от API перевода не содержит текста. Ответ: {responseStr}");
    }

    private async Task<HttpResponseMessage> SendRequestAsync(AiRequestBody? requestBody, CancellationToken token)
    {
        Log.Information("Запрос для перевода отправлен.");

        var stopWatch = Stopwatch.StartNew();

        try
        {
            var translatorResponse = await _httpClient.PostAsJsonAsync(_translatorEndPoint,
                                                                       requestBody,
                                                                       SerializationConfig.Default.AiRequestBody, token);
            stopWatch.Stop();

            Log.Information("Ответ на запрос для перевода пришёл за {ElapsedMilliseconds} мс.", stopWatch.ElapsedMilliseconds);

            return translatorResponse;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Ошибка сетевого соединения: {Message}", ex.Message);
            throw new TranslatorException("Проблема с соединением к серверу перевода", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            Log.Error(ex, "Таймаут запроса к API перевода");
            throw new TranslatorException("Сервер перевода не ответил вовремя", ex);
        }
    }

    private async Task<string> CheckResponseAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(token);
            Log.Warning("Ошибка при запросе к API: {StatusCode}", response.StatusCode);
            Log.Warning("Ответ с ошибкой: {errorResponse}", errorResponse);
            throw new TranslatorException($"Ошибка API перевода: {response.StatusCode}. {errorResponse}");
        }

        return await response.Content.ReadAsStringAsync(token);
    }

    private AiRequestBody CreateRequestBody(string text)
    {
        string sourceLang = config.LanguagePair.SourceLang;
        string targetLang = config.LanguagePair.TargetLang;
        string instruction = string.Format(config.GeminiOptions.Instructions, sourceLang, targetLang);
        var generationOptions = config.GeminiOptions.GenerationOptions;

        return new AiRequestBody(
            new GenerationConfig(generationOptions.Temperature,
                                 generationOptions.TopP,
                                 generationOptions.TopK,
                                 generationOptions.MaxOutputTokens,
                                 "text/plain"),
            [new RequestContent("user", [new RequstPart(text)])],
            new RequestContent("user", [new RequstPart(instruction)])
        );
    }
}
