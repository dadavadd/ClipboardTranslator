using ClipboardTranslator.ClipboardHandler;
using ClipboardTranslator.Configuration;
using ClipboardTranslator.Translator;


var config = Config.Load();
using var translator = new AiTranslator(config);
using var monitor = new ClipboardMonitor();
var inputSimulator = new InputSimulator();

monitor.ClipboardUpdate += async text =>
{
    ColorizeText(DateTime.UtcNow.ToString(), ConsoleColor.Green);
    ColorizeText(new string('-', 40), ConsoleColor.Green);
    ColorizeText($"ПОЛУЧЕН ТЕКСТ ИЗ БУФЕРА: {text}", ConsoleColor.Red);
    ColorizeText($"{new string('-', 40)} \n", ConsoleColor.Green);

    string? translatedText = await translator.TranslateAsync(text);

    if (string.IsNullOrEmpty(translatedText))
    {
        Console.WriteLine($"Строка {nameof(translatedText)} оказалась null");
        return;
    }

    translatedText = translatedText.TrimEnd('\n');

    Console.WriteLine(DateTime.UtcNow.ToString(), ConsoleColor.Green);
    ColorizeText(new string('-', 40), ConsoleColor.Green);
    ColorizeText($"ПЕРЕВОД: {translatedText}", ConsoleColor.Red);
    ColorizeText($"{new string('-', 40)} \n", ConsoleColor.Green);

    inputSimulator.SimulateTextInput(translatedText);
};

void ColorizeText(string text, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ResetColor();
}

Console.ReadLine();
