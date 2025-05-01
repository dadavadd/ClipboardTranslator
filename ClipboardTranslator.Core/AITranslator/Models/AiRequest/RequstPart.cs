using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.AITranslator.Models.AiRequest;

internal record class RequstPart([property: JsonPropertyName("text")] string Text);
