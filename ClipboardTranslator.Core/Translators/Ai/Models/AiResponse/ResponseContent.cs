using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;

internal record ResponseContent([property: JsonPropertyName("parts")] ResponsePart[] Parts,
                              [property: JsonPropertyName("role")] string Role);

