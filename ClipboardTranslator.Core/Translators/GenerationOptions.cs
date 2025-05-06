namespace ClipboardTranslator.Core.Translators;

public record class GenerationOptions(double TopP,
                                      int TopK,
                                      int MaxOutputTokens);
