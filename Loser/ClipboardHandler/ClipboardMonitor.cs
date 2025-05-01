using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.ComponentModel;

using static Windows.Win32.PInvoke;

namespace ClipboardTranslator.ClipboardHandler;

public unsafe class ClipboardMonitor : IDisposable
{
    private readonly PCWSTR _className;
    private HWND _hwnd;

    private Thread? _messageLoopThread;
    private uint _messageLoopThreadId;

    private const int WmClipboardUpdate = 0x031D;
    private const uint WmQuit = 0x0012;
    private const uint UnicodeText = 13;

    private static readonly HWND HWNDMessage = new(-3);

    public delegate Task ClipboardUpdateHandler(string text);
    public event ClipboardUpdateHandler? ClipboardUpdate;

    public ClipboardMonitor()
    {
        fixed (char* firstChar = "Translator_" + Guid.NewGuid())
            _className = firstChar;

        _messageLoopThread = new Thread(() =>
        {
            _messageLoopThreadId = GetCurrentThreadId();

            var wcex = new WNDCLASSEXW
            {
                cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
                lpfnWndProc = WndProc,
                hInstance = GetModuleHandle((PCWSTR)null),
                lpszClassName = _className
            };

            ushort atom = RegisterClassEx(wcex);

            if (atom == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _hwnd = CreateWindowEx(WINDOW_EX_STYLE.WS_EX_NOACTIVATE,
                                   _className,
                                   _className,
                                   WINDOW_STYLE.WS_POPUP,
                                   0,
                                   0,
                                   0,
                                   0,
                                   HWNDMessage,
                                   HMENU.Null,
                                   wcex.hInstance,
                                   null);

            if (_hwnd.IsNull)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!AddClipboardFormatListener(_hwnd))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            while (GetMessage(out var msg, HWND.Null, 0, 0) > 0)
            {
                TranslateMessage(in msg);
                DispatchMessage(in msg);
            }

        }) { IsBackground = true };

        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.Start();
    }


    private LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == WmClipboardUpdate)
        {
            string text = GetClipboardText();
            if (!string.IsNullOrEmpty(text))
                ClipboardUpdate?.Invoke(text);

            return (LRESULT)0;
        }
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    private static string GetClipboardText()
    {
        string result = string.Empty;

        if (!OpenClipboard(HWND.Null))
            return result;

        if (!IsClipboardFormatAvailable(UnicodeText))
        {
            CloseClipboard();
            return result;
        }

        var clipboardData = GetClipboardData(UnicodeText);

        if (!clipboardData.IsNull)
        {
            var dataGlobal = (HGLOBAL)clipboardData.Value;
            var dataPtr = GlobalLock(dataGlobal);

            try
            {
                result = Marshal.PtrToStringUni((nint)dataPtr) ?? string.Empty;
            }
            finally
            {
                GlobalUnlock(dataGlobal);
            }
        }

        CloseClipboard();
        return result;
    }

    public void Dispose()
    {
        RemoveClipboardFormatListener(_hwnd);
        DestroyWindow(_hwnd);
        UnregisterClass(_className, GetModuleHandle((PCWSTR)null));

        if (_messageLoopThreadId != 0)
        {
            PostThreadMessage(_messageLoopThreadId, WmQuit, 0, 0);
            _messageLoopThread?.Join();
        }
    }
}
