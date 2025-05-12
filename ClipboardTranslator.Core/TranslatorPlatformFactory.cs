using ClipboardTranslator.Core.TextUpdateHandler.Windows;
using System.Runtime.InteropServices;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Interfaces;
using ClipboardTranslator.Core.Translators.Ai;
using ClipboardTranslator.Core.Translators.Google;

namespace ClipboardTranslator.Core;

public class TranslatorPlatformFactory
{
    public static ITranslator CreateTranslator(TranslatorConfig config, CancellationToken token) => config.TranslationMode switch
    {
        "Ai" => new AiTranslator(config, token),
        "Google" => new GoogleTranslator(config, token),
        _ => throw new NotSupportedException($"Транслятор {config.TranslationMode} не реализован.")
    };

    public static ITextUpdater CreateClipboardMonitor(TranslatorConfig config, IInputSimulator inputSimulator, CancellationToken token)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return config.TranslationInputMode switch
            {
                "Clipboard" => new WindowsClipboardMonitor(inputSimulator, config, token),
                "CursorFocused" => new AutomationElementWindow(inputSimulator, config, token),
                _ => throw new ArgumentException($"Неподдерживаемый режим ввода: {config.TranslationInputMode}")
            };

        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //    return new LinuxClipboardMonitor(token);
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //    return new MacOSClipboardMonitor(token);

        throw new PlatformNotSupportedException("ClipboardMonitor пока не поддерживает вашу платформу.");
    }

    public static IInputSimulator CreateInputSimulator()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new WindowsInputSimulator();

        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //    return new LinuxInputSimulator();
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //    return new MacOSInputSimulator();

        throw new PlatformNotSupportedException("InputSimulator пока не поддерживает вашу платформу.");
    }
}
