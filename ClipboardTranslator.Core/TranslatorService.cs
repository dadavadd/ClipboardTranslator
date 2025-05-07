using ClipboardTranslator.Core.Interfaces;
using Serilog;

namespace ClipboardTranslator.Core;

public class TranslatorService : IDisposable
{
    private readonly IClipboardMonitor _monitor;
    private readonly ITranslator _translator;

    private bool _suppressClipboardUpdate;

    public TranslatorService(IClipboardMonitor monitor,
                      ITranslator translator,
                      CancellationToken token = default)
    {
        _monitor = monitor;
        _translator = translator;

        _monitor.ClipboardUpdate += OnClipboardUpdate;
    }

    private async Task OnClipboardUpdate(string text, IInputSimulator inputSimulator)
    {
        if (_suppressClipboardUpdate)
            return;

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

            inputSimulator.SimulateTextInput(translatedText);

            _suppressClipboardUpdate = true;

            inputSimulator.SetClipboardText(translatedText);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при переводе текста.");
        }
        finally
        {
            await Task.Delay(100);
            _suppressClipboardUpdate = false;
        }
    }

    public void Dispose()
    {
        _monitor.Dispose();
        Log.Information("TranslatorService.DisposeManaged вызван");
    } 
}
