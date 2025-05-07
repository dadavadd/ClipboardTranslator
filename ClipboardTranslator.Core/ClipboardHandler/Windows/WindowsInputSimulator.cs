using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using ClipboardTranslator.Core.Interfaces;
using Serilog;

using static Windows.Win32.PInvoke;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;
using static Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS;
using static Windows.Win32.UI.Input.KeyboardAndMouse.INPUT_TYPE;

namespace ClipboardTranslator.Core.ClipboardHandler.Windows;

internal unsafe class WindowsInputSimulator : IInputSimulator
{
    private const uint UnicodeText = 13;

    public string GetClipboardText()
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

            var dataGlobal = (HGLOBAL)clipboardData.Value;
            var dataPtr = GlobalLock(dataGlobal);

            return Marshal.PtrToStringUni((nint)dataPtr) ?? string.Empty;

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

    public bool SetClipboardText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        if (!OpenClipboard(HWND.Null))
        {
            Log.Warning("Не удалось открыть буфер обмена.");
            return false;
        }

        try
        {
            if (!EmptyClipboard())
            {
                Log.Warning("Не удалось очистить буфер обмена.");
                return false;
            }

            int sizeInBytes = (text.Length + 1) * sizeof(char);
            HGLOBAL hMem = GlobalAlloc(GMEM_MOVEABLE, (nuint)sizeInBytes);
            if (hMem.IsNull)
                return false;


            void* ptr = GlobalLock(hMem);
            if (ptr == null)
            {
                GlobalFree(hMem);
                return false;
            }

            fixed (char* src = text)
                Buffer.MemoryCopy(src, ptr, sizeInBytes, sizeInBytes);

            GlobalUnlock(hMem);


            if (SetClipboardData(UnicodeText, (HANDLE)hMem.Value).IsNull)
            {
                GlobalFree(hMem);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при вставке текста в буфер обмена.");
            return false;
        }
        finally
        {
            CloseClipboard();
        }
    }


    public bool SimulateTextInput(string text)
    {
        if (string.IsNullOrEmpty(text))
            return true;

        static INPUT Make(char c, KEYBD_EVENT_FLAGS flags) => new()
        {
            type = INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new()
                {
                    wScan = c,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = (nuint)nint.Zero
                }
            }
        };

        var inputs = text
            .SelectMany(static c => new[] {
                Make(c, KEYEVENTF_UNICODE),
                Make(c, KEYEVENTF_UNICODE | KEYEVENTF_KEYUP)})
            .ToArray();

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
