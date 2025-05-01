using System.Text.Json.Serialization;

namespace ClipboardTranslator.Translator.Models.AiRequest;

public record RequestBody([property: JsonPropertyName("generationConfig")] GenerationConfig GenerationConfig,
                          [property: JsonPropertyName("contents")] RequestContent[] Contents,
                          [property: JsonPropertyName("systemInstruction")] RequestContent SystemInstruction);
