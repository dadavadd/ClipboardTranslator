using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.AITranslator.Models.AiRequest;

internal record RequestContent([property: JsonPropertyName("role")] string Role,
                      [property: JsonPropertyName("parts")] RequstPart[] Parts);
