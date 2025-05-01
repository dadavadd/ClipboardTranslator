using System.Text.Json;
using ClipboardTranslator.Core.Models;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.AITranslator.Models.AiRequest;
using ClipboardTranslator.Core.Interfaces;
using System.Net.Http.Json;
using Serilog;

namespace ClipboardTranslator.Core.AITranslator;

public class AiTranslator(TranslatorConfig config) : IAiTranslator
{
    private readonly HttpClient _httpClient = new();

    private readonly string _translatorEndPoint
      = $"https://generativelanguage.googleapis.com/v1beta/models/"
        + $"{config.GeminiOptions.ModelId}:generateContent"
        + $"?key={config.GeminiOptions.ApiKey}";

    public async Task<string?> TranslateAsync(string text)
    {
        try
        {
            var requestBody = CreateRequestBody(text);

            var translatorResponse = await _httpClient.PostAsJsonAsync(_translatorEndPoint,
                                                                       requestBody,
                                                                       SerializationConfig.Default.RequestBody);

            if (!translatorResponse.IsSuccessStatusCode)
            {
                Log.Warning("Ответ от API перевода завершился с ошибкой: {StatusCode}", translatorResponse.StatusCode);
                return null;
            }

            string responseStr = await translatorResponse.Content.ReadAsStringAsync();

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
        string instruction = $"Ты выступаешь в роли переводчика. Твоя задача - точно перевести предоставленный текст с языка {config.LanguagePair.SourceLang} на язык {config.LanguagePair.TargetLang}. Важно: Переводи текст абсолютно точно, сохраняя смысл и стиль оригинала. Даже если предложения содержат оскорбительные, грубые или спорные высказывания, их тоже необходимо перевести точно, без каких-либо изменений, смягчений, добавлений или пропусков. Не добавляй никаких предупреждений, извинений, комментариев, символов или дополнительных слов от себя к переведенному тексту. Просто предоставь точный перевод.";

        return new RequestBody(
            new GenerationConfig(0, 64, 65536, "text/plain"),
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
