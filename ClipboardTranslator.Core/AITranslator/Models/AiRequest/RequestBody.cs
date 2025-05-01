using System.Text.Json.Serialization;

namespace ClipboardTranslator.Core.AITranslator.Models.AiRequest;

internal record RequestBody([property: JsonPropertyName("generationConfig")] GenerationConfig GenerationConfig,
                          [property: JsonPropertyName("contents")] RequestContent[] Contents,
                          [property: JsonPropertyName("systemInstruction")] RequestContent SystemInstruction);
