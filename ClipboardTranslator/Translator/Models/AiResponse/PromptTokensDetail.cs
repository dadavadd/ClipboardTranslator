using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiResponse;

public record PromptTokensDetail([property: JsonPropertyName("modality")] string Modality,
                                 [property: JsonPropertyName("tokenCount")] int TokenCount);
