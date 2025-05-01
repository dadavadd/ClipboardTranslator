namespace ClipboardTranslator.Core.Interfaces;

public interface IAiTranslator : IDisposable
{
    Task<string?> TranslateAsync(string input);
}
