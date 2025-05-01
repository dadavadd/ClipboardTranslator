using ClipboardTranslator.Core.ClipboardHandler;
using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.AITranslator;
using ClipboardTranslator.Core;
using Serilog;

try
{
    Console.Title = "https://github.com/dadavadd/ClipboardTranslator";

    SetConsoleLoggingOption();

    var config = TranslatorConfig.Load();

    using var translator = new Translator(
        new ClipboardMonitor(),
        new AiTranslator(config)
    );

    Console.ReadLine();
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred: {Message}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}

void SetConsoleLoggingOption()
{
    Console.WriteLine("Выводить логи в консоль?: [y/n]");
    Console.WriteLine("y - да");
    Console.WriteLine("n - нет");
    Console.WriteLine("По умолчанию - нет");

    var pressedKey = Console.ReadKey().Key;

    switch (pressedKey)
    {
        case ConsoleKey.Y:
            LogConfig.Configure();
            break;
        case ConsoleKey.N:
            LogConfig.Configure(useConsole: false);
            break;
        default:
            Console.WriteLine("Неверный ввод. Вывод логов в консоль отключен.");
            LogConfig.Configure(useConsole: false);
            break;
    }

    Console.Clear();
}



