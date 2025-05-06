namespace ClipboardTranslator.Core.Interfaces;

public interface IClipboardMonitor : IDisposable
{
    event Func<string, IInputSimulator, Task>? ClipboardUpdate;
}
