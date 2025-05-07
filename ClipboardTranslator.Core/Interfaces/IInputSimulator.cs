namespace ClipboardTranslator.Core.Interfaces;

public interface IInputSimulator
{
    string GetClipboardText();
    bool SetClipboardText(string text);
    bool SimulateTextInput(string text);
}
