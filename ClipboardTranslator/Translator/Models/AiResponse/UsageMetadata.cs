using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiResponse;

public record UsageMetadata([property: JsonPropertyName("promptTokenCount")] int PromptTokenCount,
                            [property: JsonPropertyName("candidatesTokenCount")] int CandidatesTokenCount,
                            [property: JsonPropertyName("totalTokenCount")] int TotalTokenCount,
                            [property: JsonPropertyName("promptTokensDetails")] PromptTokensDetail[] PromptTokensDetails,
                            [property: JsonPropertyName("thoughtsTokenCount")] int ThoughtsTokenCount);

