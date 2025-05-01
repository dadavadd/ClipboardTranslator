using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.AITranslator.Models.AiResponse;

internal record Candidate([property: JsonPropertyName("content")] ResponseContent Content,
                        [property: JsonPropertyName("finishReason")] string FinishReason,
                        [property: JsonPropertyName("index")] int Index);
