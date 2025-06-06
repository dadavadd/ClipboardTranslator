﻿using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.Translators.Ai.Models.AiResponse;

internal record AiResponseBody([property: JsonPropertyName("candidates")] Candidate[] Candidates,
                       [property: JsonPropertyName("usageMetadata")] UsageMetadata UsageMetadata,
                       [property: JsonPropertyName("modelVersion")] string ModelVersion);
