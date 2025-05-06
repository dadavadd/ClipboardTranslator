using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;

internal record Candidate([property: JsonPropertyName("content")] ResponseContent Content,
                        [property: JsonPropertyName("finishReason")] string FinishReason,
                        [property: JsonPropertyName("index")] int Index);
