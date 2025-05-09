using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Exceptions;
using ClipboardTranslator.Core;
using Serilog;

try
{
    Console.Title = "https://github.com/dadavadd/ClipboardTranslator";

    SetConsoleLoggingOption();
    StartTranslator();
}
catch (ConfigException ex)
{
    Log.Fatal(ex, "Ошибка конфигурации: {Message}", ex.Message);
    Console.WriteLine($"Детали: {ex?.InnerException?.Message}");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Критическая ошибка: {Message}", ex.Message);
}
finally
{
    Console.WriteLine("Нажмите любую клавишу для выхода...");
    Console.ReadKey();
    Log.CloseAndFlush();
}

static void SetConsoleLoggingOption()
{
    Console.WriteLine("Выводить логи в консоль?: [y/n]");
    Console.WriteLine("y - да");
    Console.WriteLine("n - нет");
    Console.WriteLine("По умолчанию - нет");

    try
    {
        var pressedKey = Console.ReadKey(intercept: true).Key;

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
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Ошибка при настройке логирования");
        LogConfig.Configure(useConsole: false);
    }

    Console.Clear();
}

static void StartTranslator()
{
    var config = TranslatorConfig.Load();
    var cts = new CancellationTokenSource();

    var inputSimulator = TranslatorPlatformFactory.CreateInputSimulator();
    var clipboardMonitor = TranslatorPlatformFactory.CreateClipboardMonitor(inputSimulator, cts.Token);
    var translator = TranslatorPlatformFactory.CreateTranslator(config, cts.Token);

    using var translatorService = new TranslatorService(clipboardMonitor, translator);

    Console.WriteLine("Переводчик запущен. Нажмите Enter для завершения работы.");
    Console.ReadLine();

    cts.Cancel();
}