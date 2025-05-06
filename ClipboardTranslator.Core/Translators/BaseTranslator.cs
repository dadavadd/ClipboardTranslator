namespace ClipboardTranslator.Core.Translators;

public abstract class BaseTranslator : DisposableBase
{
    public abstract Task<string?> TranslateAsync(string text);
}
