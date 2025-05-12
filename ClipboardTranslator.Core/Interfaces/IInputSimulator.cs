namespace ClipboardTranslator.Core.Interfaces;

public interface IInputSimulator
{
    string GetClipboardText();
    string CopyAndGetClipboardText();
    bool SetClipboardText(string text);
    bool SimulateTextInput(string text);
}
