using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;

internal record ResponsePart([property: JsonPropertyName("text")] string Text);
