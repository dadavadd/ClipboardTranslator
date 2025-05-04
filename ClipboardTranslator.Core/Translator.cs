using ClipboardTranslator.Core.ClipboardHandler;
using ClipboardTranslator.Core.Interfaces;
using Serilog;

namespace ClipboardTranslator.Core;

public class Translator : IDisposable
{
    private readonly IClipboardMonitor _monitor;
    private readonly ITranslator _translator;

    public Translator(IClipboardMonitor monitor, ITranslator translator)
    {
        _monitor = monitor;
        _translator = translator;

        _monitor.ClipboardUpdate += OnClipboardUpdate;
    }

    private async Task OnClipboardUpdate(string text)
    {
        try
        {
            Log.Information($"Получен текст из буфера обмена: {text}");
            string? translatedText = await _translator.TranslateAsync(text);
            if (string.IsNullOrEmpty(translatedText))
            {
                Log.Warning("Переведённый текст оказался пустым или null.");
                return;
            }

            translatedText = translatedText.TrimEnd('\n');
            Log.Information($"Перевод завершён: {text}");

            InputSimulator.SimulateTextInput(translatedText);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при переводе текста.");
        }
    }

    public void Dispose()
    {
        _monitor.Dispose();
        _translator.Dispose();
    }
}
