using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiResponse;

public record Candidate([property: JsonPropertyName("content")] ResponseContent Content,
                        [property: JsonPropertyName("finishReason")] string FinishReason,
                        [property: JsonPropertyName("index")] int Index);
