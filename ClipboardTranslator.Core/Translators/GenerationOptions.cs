namespace ClipboardTranslator.Core.Translators;

public class GenerationOptions
{
    public required double Temperature { get; set; }
    public required double TopP { get; set; }
    public required int TopK { get; set; }
    public required int MaxOutputTokens { get; set; }
}
