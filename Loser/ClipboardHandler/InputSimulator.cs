using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.CompilerServices;

using static Windows.Win32.PInvoke;
using static Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS;
using static Windows.Win32.UI.Input.KeyboardAndMouse.INPUT_TYPE;


namespace ClipboardTranslator.ClipboardHandler;

public class InputSimulator
{
    public bool SimulateTextInput(string text)
    {
        if (string.IsNullOrEmpty(text))
            return true;

        char[] chars = text.ToCharArray();

        INPUT[] inputs = new INPUT[chars.Length * 2];

        int inputIndex = 0;

        foreach (char c in chars)
        {
            inputs[inputIndex].type = INPUT_KEYBOARD;
            inputs[inputIndex].Anonymous.ki.wScan = c;
            inputs[inputIndex].Anonymous.ki.dwFlags = KEYEVENTF_UNICODE;
            inputs[inputIndex].Anonymous.ki.time = 0;
            inputs[inputIndex].Anonymous.ki.dwExtraInfo = (nuint)IntPtr.Zero;
            inputIndex++;

            inputs[inputIndex].type = INPUT_KEYBOARD;
            inputs[inputIndex].Anonymous.ki.wScan = c;
            inputs[inputIndex].Anonymous.ki.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;
            inputs[inputIndex].Anonymous.ki.time = 0;
            inputs[inputIndex].Anonymous.ki.dwExtraInfo = (nuint)IntPtr.Zero;
            inputIndex++;
        }

        unsafe
        {
            fixed (INPUT* pInputs = inputs)
            {
                uint result = SendInput((uint)inputs.Length, pInputs, Unsafe.SizeOf<INPUT>());
            }
            
        }

        return true;
    }
}
