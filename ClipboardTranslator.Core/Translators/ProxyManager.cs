using System.Net;
using System.Text.RegularExpressions;
using ClipboardTranslator.Core.Configuration;
using Serilog;

namespace ClipboardTranslator.Core.Translators;

internal partial class ProxyManager
{
    public static void SetProxyIfNeeded(TranslatorConfig config)
    {
        var proxy = config.Proxy;

        if (proxy == "None")
        {
            Log.Information("Прокси не используется.");
            return;
        }

        if (!ProxyCheckRegex().Match(proxy).Success)
            throw new FormatException("Неверный формат прокси. Требуется: protocol://[user:pass@]host[:port]");

        var proxyUri = new Uri(proxy);
        HttpClient.DefaultProxy = new WebProxy(proxyUri);

        Log.Information("Прокси используется: {proxy}", proxyUri.Host);
    }

    [GeneratedRegex(@"^(http|https|socks4|socks5)://([^:]+:[^@]+@)?([a-zA-Z0-9.-]+)(:\d+)?$", RegexOptions.IgnoreCase, "ru-RU")]
    private static partial Regex ProxyCheckRegex();
}
