using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Translators;
using ClipboardTranslator.Core.Translators.Ai;
using ClipboardTranslator.Core.Translators.Google;

namespace ClipboardTranslator.Core;

public class TranslatorFactory
{
    public static BaseTranslator CreateTranslator(TranslatorConfig config, CancellationToken token = default) => config.TranslationMode switch
    {
        "Ai" => new AiTranslator(config, token),
        "Google" => new GoogleTranslator(config, token),
        _ => throw new NotSupportedException($"Транслятор {config.TranslationMode} не реализован.")
    };
}
