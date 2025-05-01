using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiRequest;

public record class RequstPart([property: JsonPropertyName("text")] string Text);
