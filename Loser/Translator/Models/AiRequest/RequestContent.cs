using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiRequest;

public record RequestContent([property: JsonPropertyName("role")] string Role,
                      [property: JsonPropertyName("parts")] RequstPart[] Parts);
