namespace ClipboardTranslator.Core.Interfaces;

public interface ITranslator : IDisposable
{
    Task<string?> TranslateAsync(string text);
}
