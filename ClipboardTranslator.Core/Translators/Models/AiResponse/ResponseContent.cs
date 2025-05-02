using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Models.AiResponse;

internal record ResponseContent([property: JsonPropertyName("parts")] ResponsePart[] Parts,
                              [property: JsonPropertyName("role")] string Role);

