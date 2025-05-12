using ClipboardTranslator.Core.Configuration;
using ClipboardTranslator.Core.Exceptions;
using ClipboardTranslator.Core;
using Serilog;

internal static class Program
{
    private static void Main()
    {
        try
        {
            Console.Title = "https://github.com/dadavadd/ClipboardTranslator";
            InitializeApplication();
        }
        catch (ConfigException ex)
        {
            Log.Fatal(ex, "Ошибка конфигурации: {Message}", ex.Message);
            Console.WriteLine($"Подробности: {ex?.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Критическая ошибка: {Message}", ex.Message);
        }
        finally
        {
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadKey();
            Log.CloseAndFlush();
        }
    }

    private static void InitializeApplication()
    {
        ConfigureLogging();
        StartTranslatorService();
    }

    private static void ConfigureLogging()
    {
        Console.WriteLine("Включить логирование в консоль? [y/n]");
        Console.WriteLine("y - да\nn - нет\nПо умолчанию - нет");

        var key = Console.ReadKey(intercept: true).Key;

        try
        {
            LogConfig.Configure(key == ConsoleKey.Y);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Ошибка при настройке логирования");
            LogConfig.Configure(false);
        }

        Console.Clear();
    }

    private static void StartTranslatorService()
    {
        var config = TranslatorConfig.Load();
        using var cts = new CancellationTokenSource();

        var inputSimulator = TranslatorPlatformFactory.CreateInputSimulator();
        var clipboardMonitor = TranslatorPlatformFactory.CreateClipboardMonitor(config, inputSimulator, cts.Token);
        var translator = TranslatorPlatformFactory.CreateTranslator(config, cts.Token);

        using var translatorService = new TranslatorService(clipboardMonitor, translator);

        Console.WriteLine("Переводчик запущен. Нажмите Enter для выхода.");
        Console.ReadLine();

        cts.Cancel();
    }
}