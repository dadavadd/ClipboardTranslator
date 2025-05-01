using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Serilog;

using static Windows.Win32.PInvoke;
using static Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS;
using static Windows.Win32.UI.Input.KeyboardAndMouse.INPUT_TYPE;

namespace ClipboardTranslator.Core.ClipboardHandler;

public class InputSimulator
{
    private const uint UnicodeText = 13;
    internal static string GetClipboardText()
    {
        if (!OpenClipboard(HWND.Null))
        {
            Log.Warning("Не удалось открыть буфер обмена.");
            return string.Empty;
        }

        try
        {
            if (!IsClipboardFormatAvailable(UnicodeText))
            {
                Log.Warning("Формат UnicodeText не найден в буфере обмена.");
                return string.Empty;
            }

            var clipboardData = GetClipboardData(UnicodeText);
            if (clipboardData.IsNull)
            {
                Log.Warning("Буфер обмена пуст.");
                return string.Empty;
            }

            unsafe
            {
                var dataGlobal = (HGLOBAL)clipboardData.Value;
                var dataPtr = GlobalLock(dataGlobal);

                return Marshal.PtrToStringUni((nint)dataPtr) ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при чтении текста из буфера обмена.");
            return string.Empty;
        }
        finally
        {
            CloseClipboard();
        }
    }

    internal static bool SimulateTextInput(string text)
    {
        if (string.IsNullOrEmpty(text))
            return true;

        char[] chars = text.ToCharArray();

        INPUT[] inputs = new INPUT[chars.Length * 2];

        int inputIndex = 0;

        foreach (var c in chars)
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
                uint result = SendInput((uint)inputs.Length, pInputs, sizeof(INPUT));
            }
        }

        return true;
    }
}
