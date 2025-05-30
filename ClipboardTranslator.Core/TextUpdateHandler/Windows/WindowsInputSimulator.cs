﻿using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using ClipboardTranslator.Core.Interfaces;
using Serilog;

using static Windows.Win32.PInvoke;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;
using static Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS;
using static Windows.Win32.UI.Input.KeyboardAndMouse.INPUT_TYPE;
using static Windows.Win32.UI.Input.KeyboardAndMouse.VIRTUAL_KEY;

namespace ClipboardTranslator.Core.TextUpdateHandler.Windows;

internal unsafe class WindowsInputSimulator : IInputSimulator
{
    private const uint UnicodeText = 13;

    public string CopyAndGetClipboardText()
    {
        var inputs = new List<INPUT>
        {
            MakeVirtualKey(VK_CONTROL, 0),
            MakeVirtualKey(VK_C, 0),
            MakeVirtualKey(VK_C, KEYEVENTF_KEYUP),
            MakeVirtualKey(VK_CONTROL, KEYEVENTF_KEYUP)
        };

        fixed (INPUT* pInputs = inputs.ToArray())
        {
            SendInput((uint)inputs.Count, pInputs, sizeof(INPUT));
        }

        Task.Delay(20).Wait();

        string text = GetClipboardText();

        if (!string.IsNullOrEmpty(text))
            ClearClipboard();

        return text;
    }

    private bool ClearClipboard()
    {
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
            return true;
        }
        finally
        {
            CloseClipboard();
        }
    }

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
        finally
        {
            CloseClipboard();
        }
    }

    public bool SetClipboardText(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

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

            if (string.IsNullOrEmpty(text))
                return true;

            int sizeInBytes = (text.Length + 1) * sizeof(char);
            HGLOBAL hMem = GlobalAlloc(GMEM_MOVEABLE, (nuint)sizeInBytes);
            if (hMem.IsNull)
                throw new InvalidOperationException("Не удалось выделить память для текста.");

            void* ptr = GlobalLock(hMem);
            if (ptr == null)
            {
                GlobalFree(hMem);
                throw new InvalidOperationException("Не удалось зафиксировать память.");
            }

            fixed (char* src = text)
                Buffer.MemoryCopy(src, ptr, sizeInBytes, sizeInBytes);

            GlobalUnlock(hMem);

            if (SetClipboardData(UnicodeText, (HANDLE)hMem.Value).IsNull)
            {
                GlobalFree(hMem);
                throw new InvalidOperationException("Не удалось установить данные в буфер обмена.");
            }

            return true;
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

        static INPUT MakeUnicode(char c, KEYBD_EVENT_FLAGS flags) => new()
        {
            type = INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new()
                {
                    wScan = c,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = 0
                }
            }
        };

        var inputs = new List<INPUT>();

        foreach (char c in text)
        {
            if (c == '\n')
            {
                inputs.Add(MakeVirtualKey(VK_LSHIFT, 0));
                inputs.Add(MakeVirtualKey(VK_RETURN, 0));
                inputs.Add(MakeVirtualKey(VK_RETURN, KEYEVENTF_KEYUP));
                inputs.Add(MakeVirtualKey(VK_LSHIFT, KEYEVENTF_KEYUP));
            }
            else
            {
                inputs.Add(MakeUnicode(c, KEYEVENTF_UNICODE));
                inputs.Add(MakeUnicode(c, KEYEVENTF_UNICODE | KEYEVENTF_KEYUP));
            }
        }

        fixed (INPUT* pInputs = inputs.ToArray())
        {
            uint result = SendInput((uint)inputs.Count, pInputs, sizeof(INPUT));
            if (result == 0)
                throw new InvalidOperationException("Не удалось отправить симулированный ввод.");
        }


        return true;
    }

    private static INPUT MakeVirtualKey(VIRTUAL_KEY vk, KEYBD_EVENT_FLAGS flags) => new()
    {
        type = INPUT_KEYBOARD,
        Anonymous = new INPUT._Anonymous_e__Union
        {
                ki = new()
                {
                    wVk = vk,
                    dwFlags = flags,
                    time = 0,
                dwExtraInfo = 0
            }
        }
    };
}