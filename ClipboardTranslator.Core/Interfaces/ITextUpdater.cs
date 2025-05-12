namespace ClipboardTranslator.Core.Interfaces;

public interface ITextUpdater : IDisposable
{
    event Func<string, IInputSimulator, Task>? TextUpdate;
}
