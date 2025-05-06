namespace ClipboardTranslator.Core.Interfaces;

public interface IClipboardMonitor
{
    event Func<string, Task>? ClipboardUpdate;
    void Dispose();
}
