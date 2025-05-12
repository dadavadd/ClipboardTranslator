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

        if (proxy.Equals("None", StringComparison.OrdinalIgnoreCase))
        {
            Log.Information("Прокси не используется.");
            return;
        }

        var proxyUri = new Uri(proxy);

        try
        {
            HttpClient.DefaultProxy = new WebProxy(proxyUri);
        }
        catch (UriFormatException ex)
        {
            Log.Error(ex, "Неверный формат прокси. Требуется: protocol://[user:pass@]host[:port]");
            throw;
        }

        Log.Information("Прокси используется: {proxy}", proxyUri.Host);
    }
}
