using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiResponse;

public record ResponseContent([property: JsonPropertyName("parts")] ResponsePart[] Parts,
                              [property: JsonPropertyName("role")] string Role);

