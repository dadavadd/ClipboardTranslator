namespace ClipboardTranslator.Core.Translators;

public record class GenerationOptions(double Temperature,
                                      double TopP,
                                      int TopK,
                                      int MaxOutputTokens);
