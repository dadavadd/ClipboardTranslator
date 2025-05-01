using ClipboardTranslator.Core.ClipboardHandler;

namespace ClipboardTranslator.Core.Interfaces;

public interface IClipboardMonitor : IDisposable
{
    event ClipboardMonitor.ClipboardUpdateHandler ClipboardUpdate;
}
