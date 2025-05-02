using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Models.AiRequest;

internal record class RequstPart([property: JsonPropertyName("text")] string Text);
