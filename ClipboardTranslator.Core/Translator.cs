using ClipboardTranslator.Core.Interfaces;
using Serilog;

namespace ClipboardTranslator.Core;

public class Translator : IDisposable
{
    private readonly IClipboardMonitor _monitor;
    private readonly IInputSimulator _simulator;
    private readonly IAiTranslator _translator;

    public Translator(IClipboardMonitor monitor, IInputSimulator simulator, IAiTranslator translator)
    {
        _monitor = monitor;
        _simulator = simulator;
        _translator = translator;

        _monitor.ClipboardUpdate += OnClipboardUpdate;
    }

    private async Task OnClipboardUpdate(string text)
    {
        Log.Information("Получен текст из буфера обмена: {Text}", text);

        string? translatedText = await _translator.TranslateAsync(text);

        if (string.IsNullOrEmpty(translatedText))
        {
            Log.Warning("Переведённый текст оказался пустым или null.");
            return;
        }

        translatedText = translatedText.TrimEnd('\n');

        Log.Information("Перевод завершён: {TranslatedText}", translatedText);

        _simulator.SimulateTextInput(translatedText);
    }

    public void Dispose()
    {
        _monitor.Dispose();
        _translator.Dispose();
    }
}
