using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.AITranslator.Models.AiResponse;

internal record Response([property: JsonPropertyName("candidates")] Candidate[] Candidates,
                       [property: JsonPropertyName("usageMetadata")] UsageMetadata UsageMetadata,
                       [property: JsonPropertyName("modelVersion")] string ModelVersion);
