using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiResponse;

public record Response([property: JsonPropertyName("candidates")] Candidate[] Candidates,
                       [property: JsonPropertyName("usageMetadata")] UsageMetadata UsageMetadata,
                       [property: JsonPropertyName("modelVersion")] string ModelVersion);
