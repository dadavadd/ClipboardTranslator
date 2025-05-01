using Serilog;

namespace ClipboardTranslator.Core.Configuration;

public class LogConfig
{
    public static void Configure(bool useConsole = true)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug();

        if (useConsole)
            loggerConfig.WriteTo.Console();

        loggerConfig.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);

        Log.Logger = loggerConfig.CreateLogger();
    }
}
