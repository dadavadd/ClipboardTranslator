using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.AITranslator.Models.AiResponse;

internal record ResponsePart([property: JsonPropertyName("text")] string Text);
