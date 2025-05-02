using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Models.AiRequest;

internal record GenerationConfig([property: JsonPropertyName("topP")] double TopP,
                               [property: JsonPropertyName("topK")] int TopK,
                               [property: JsonPropertyName("maxOutputTokens")] int MaxOutputTokens,
                               [property: JsonPropertyName("responseMimeType")] string? ResponseMimeType);
