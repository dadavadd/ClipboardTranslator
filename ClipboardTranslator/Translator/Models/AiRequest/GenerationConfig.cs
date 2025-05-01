using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiRequest;

public record GenerationConfig([property: JsonPropertyName("topP")] double TopP,
                               [property: JsonPropertyName("topK")] int TopK,
                               [property: JsonPropertyName("maxOutputTokens")] int MaxOutputTokens,
                               [property: JsonPropertyName("responseMimeType")] string? ResponseMimeType);
