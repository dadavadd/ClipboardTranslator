using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Interfaces;
using Serilog;

namespace ClipboardTranslator.Core.Translators;

public abstract class BaseTranslator<TResponse>(TranslatorConfig config,
                                                HttpClient? httpClient = null) : ITranslator
{
    protected readonly HttpClient Client = httpClient ?? InitializeHttpClient(httpClient);
    protected readonly TranslatorConfig Config  = config;

    public async Task<string?> TranslateAsync(string text)
    {
        try
        {
            var requestBody = CreateRequestBody(text);
            using var translatorResponse = await SendRequestAsync(requestBody);
            string? responseStr = await ReadResponseContentAsync(translatorResponse);

            if (responseStr == null)
            {
                Log.Warning("Ответ от API перевода пустой.");
                return null;
            }
            var response = JsonSerializer.Deserialize(responseStr, GetJsonTypeInfo());

            if (response is not TResponse parsed)
            {
                Log.Warning("Не удалось десериализовать ответ от API перевода.");
                return null;
            }

            return ExtractText(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при переводе текста.");
            return null;
        }
    }

    private static HttpClient InitializeHttpClient(HttpClient? httpClient)
        => httpClient ?? new(new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 20,
            PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            KeepAlivePingDelay = TimeSpan.FromSeconds(30),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(10),

            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
        })
        {
            DefaultRequestVersion = new Version(2, 0)
        };

    private async Task<string?> ReadResponseContentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Log.Warning("Ошибка при запросе к API: {StatusCode}", response.StatusCode);
            Log.Warning("Ответ с ошибкой: {ErrorContent}", content);
            return null;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            Log.Warning("Ответ от API перевода пустой.");
            return null;
        }

        return content;
    }

    protected abstract object CreateRequestBody(string text);
    protected abstract Task<HttpResponseMessage> SendRequestAsync(object requestBody);
    protected abstract string? ExtractText(TResponse response);
    protected abstract JsonTypeInfo<TResponse> GetJsonTypeInfo();

    public void Dispose()
    {
        Client?.Dispose();
    }
}
