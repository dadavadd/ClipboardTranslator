namespace ClipboardTranslator.Core.Translators;

public class GeminiOptions
{
    public required string ApiKey { get; set; }
    public required string ModelId { get; set; }
    public required GenerationOptions GenerationOptions { get; set; }
    public required string Instructions { get; set; }
}
