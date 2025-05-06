namespace ClipboardTranslator.Core.Interfaces;

public interface IInputSimulator
{
    string GetClipboardText();
    bool SimulateTextInput(string text);
}
