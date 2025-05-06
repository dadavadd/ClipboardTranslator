namespace ClipboardTranslator.Core.Translators;

public record GeminiOptions(string ApiKey,
                            string ModelId,
                            GenerationOptions GenerationOptions,
                            string Instructions);
