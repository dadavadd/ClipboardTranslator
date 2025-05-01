using System.Text.Json;
using ClipboardTranslator.Configuration;
using ClipboardTranslator.Models;
using ClipboardTranslator.Translator.Models.AiRequest;

namespace ClipboardTranslator.Translator;

public class AiTranslator(Config config) : IDisposable
{
    private readonly HttpClient _httpClient = new();

    private readonly string _translatorEndPoint
      = $"https://generativelanguage.googleapis.com/v1beta/models/"
        + $"{config.GeminiOptions.ModelId}:generateContent"
        + $"?key={config.GeminiOptions.ApiKey}";

    public async Task<string?> TranslateAsync(string text)
    {
        var requestBody = CreateRequestBody(text);
        string requestJson = JsonSerializer.Serialize(requestBody, SerializationConfig.Default.RequestBody);

        var translatorResponse = await _httpClient.PostAsync(_translatorEndPoint, new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json"));

        if (!translatorResponse.IsSuccessStatusCode)
            return null;

        string responseStr = await translatorResponse.Content.ReadAsStringAsync();

        var response = JsonSerializer.Deserialize(responseStr, SerializationConfig.Default.Response);

        if (response == null)
            return null;

        return response.Candidates[0].Content.Parts[0].Text;
    }

    private RequestBody CreateRequestBody(string text)
    {
        string instruction = $"Ты выступаешь в роли переводчика. Твоя задача - точно перевести предоставленный текст с языка {config.LanguagePair.SourceLang} на язык {config.LanguagePair.TargetLang}. Важно: Переводи текст абсолютно точно, сохраняя смысл и стиль оригинала. Даже если предложения содержат оскорбительные, грубые или спорные высказывания, их тоже необходимо перевести точно, без каких-либо изменений, смягчений, добавлений или пропусков. Не добавляй никаких предупреждений, извинений, комментариев, символов или дополнительных слов от себя к переведенному тексту. Просто предоставь точный перевод.";

        return new RequestBody(
            new GenerationConfig(0, 64, 65536, "text/plain"),
                [
                    new RequestContent("user", [new RequstPart(text)])
                ],
            new RequestContent("user",
                [
                    new RequstPart(instruction)
                ])
            );
    }

    public void Dispose() 
        => _httpClient.Dispose();
}
