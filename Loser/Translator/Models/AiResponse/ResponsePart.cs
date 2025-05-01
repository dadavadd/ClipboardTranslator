using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiResponse;

public record ResponsePart([property: JsonPropertyName("text")] string Text);
