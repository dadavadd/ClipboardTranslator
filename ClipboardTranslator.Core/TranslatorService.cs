using ClipboardTranslator.Core.ClipboardHandler;
using ClipboardTranslator.Core.Interfaces;
using ClipboardTranslator.Core.Translators;
using Serilog;

namespace ClipboardTranslator.Core;

public class TranslatorService : DisposableBase
{
    private readonly IClipboardMonitor _monitor;
    private readonly BaseTranslator _translator;

    public TranslatorService(IClipboardMonitor monitor,
                      BaseTranslator translator,
                      CancellationToken token = default)
    {
        _monitor = monitor;
        _translator = translator;

        _monitor.ClipboardUpdate += OnClipboardUpdate;
    }

    private async Task OnClipboardUpdate(string text)
    {
        try
        {
            Log.Information("Получен текст из буфера обмена: {text}", text);
            string? translatedText = await _translator.TranslateAsync(text);
            if (string.IsNullOrEmpty(translatedText))
            {
                Log.Warning("Переведённый текст оказался пустым или null.");
                return;
            }

            translatedText = translatedText.TrimEnd('\n', '\r');
            Log.Information("Перевод завершён: {translatedText}", translatedText);

            InputSimulator.SimulateTextInput(translatedText);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при переводе текста.");
        }
    }

    protected override void DisposeManaged()
    {
        _monitor.Dispose();
        _translator.Dispose();
        Log.Information("TranslatorService.DisposeManaged вызван");
    } 
}
