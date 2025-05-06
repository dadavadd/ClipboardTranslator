using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;

internal record PromptTokensDetail([property: JsonPropertyName("modality")] string Modality,
                                 [property: JsonPropertyName("tokenCount")] int TokenCount);
