using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.ComponentModel;
using ClipboardTranslator.Core.Interfaces;
using Serilog;

using static Windows.Win32.PInvoke;

namespace ClipboardTranslator.Core.ClipboardHandler;

public unsafe class ClipboardMonitor : IClipboardMonitor
{
    private readonly PCWSTR _className;

    private HWND _hwnd;
    private Thread? _messageLoopThread;
    private uint _messageLoopThreadId;

    private const int WmClipboardUpdate = 0x031D;
    private const uint WmQuit = 0x0012;

    private static readonly HWND HWNDMessage = new(-3);

    public event Func<string, Task>? ClipboardUpdate;

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

            ushort atom = RegisterClassEx(in wcex);

            if (atom == 0)
            {
                var error = Marshal.GetLastWin32Error();
                Log.Error("Не удалось зарегистрировать класс окна: {ErrorCode}", error);
                throw new Win32Exception(error);
            }

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
            {
                var error = Marshal.GetLastWin32Error();
                Log.Error("Не удалось создать окно: {ErrorCode}", error);
                throw new Win32Exception(error);
            }

            if (!AddClipboardFormatListener(_hwnd))
            {
                var error = Marshal.GetLastWin32Error();
                Log.Error("Не удалось подписаться на обновления буфера обмена: {ErrorCode}", error);
                throw new Win32Exception(error);
            }

            Log.Information("Класс ClipboardMonitor успешно инициализирован.");

            while (GetMessage(out var msg, HWND.Null, 0, 0) > 0)
            {
                TranslateMessage(in msg);
                DispatchMessage(in msg);
            }

            Log.Information("Цикл обработки сообщений ClipboardMonitor завершён.");

        }) { IsBackground = true };

        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.Start();
    }

    private LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == WmClipboardUpdate)
        {
            string text = InputSimulator.GetClipboardText();
            if (!string.IsNullOrEmpty(text))
                _ = ClipboardUpdate?.Invoke(text);

            return (LRESULT)0;
        }
        return DefWindowProc(hwnd, msg, wParam, lParam);
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

        Log.Information($"Вызван Dispose у {nameof(ClipboardMonitor)}");
    }
}
