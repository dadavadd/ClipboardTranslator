using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.ComponentModel;
using ClipboardTranslator.Core.Interfaces;
using Serilog;

using static Windows.Win32.PInvoke;

namespace ClipboardTranslator.Core.ClipboardHandler.Windows;

public unsafe class WindowsClipboardMonitor : DisposableBase, IClipboardMonitor
{
    private readonly IInputSimulator _inputSimulator;

    private readonly PCWSTR _className;
    private readonly CancellationToken _token;

    private HWND _hwnd;
    private CancellationTokenRegistration _tokenRegistration;
    private Thread? _messageLoopThread;
    private uint _messageLoopThreadId;

    private const int WmClipboardUpdate = 0x031D;
    private const uint WmQuit = 0x0012;

    private static readonly HWND HWNDMessage = new(-3);

    public event Func<string, IInputSimulator, Task>? ClipboardUpdate;

    public WindowsClipboardMonitor(IInputSimulator inputSimulator, CancellationToken token = default)
    {
        _token = token;
        _inputSimulator = inputSimulator;

        fixed (char* firstChar = ("Translator_" + Guid.NewGuid()).ToCharArray())
            _className = firstChar;

        StartMessageThread();
    }

    private void StartMessageThread()
    {
        _messageLoopThread = new(() =>
        {
            _messageLoopThreadId = GetCurrentThreadId();

            _tokenRegistration = _token.Register(() =>
                PostThreadMessage(_messageLoopThreadId, WmQuit, 0, 0));

            InitializeWindow();
            RunMessageLoop();
        })
        {
            IsBackground = true
        };

        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.Start();
    }

    private void InitializeWindow()
    {
        RegisterWindowClass();
        CreateMessageWindow();

        if (!AddClipboardFormatListener(_hwnd))
            ThrowLastWin32Exception("Не удалось подписаться на обновления буфера обмена");

        Log.Information("Класс ClipboardMonitor успешно инициализирован.");
    }

    private void RegisterWindowClass()
    {
        var wcex = new WNDCLASSEXW
        {
            cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
            lpfnWndProc = WndProc,
            hInstance = GetModuleHandle((PCWSTR)null),
            lpszClassName = _className
        };

        if (RegisterClassEx(in wcex) == 0)
            ThrowLastWin32Exception("Не удалось зарегистрировать класс окна");
    }

    private void CreateMessageWindow()
    {
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
                               GetModuleHandle((PCWSTR)null),
                               null);

        if (_hwnd.IsNull)
            ThrowLastWin32Exception("Не удалось создать окно");
    }

    private static void ThrowLastWin32Exception(string message)
    {
        int error = Marshal.GetLastWin32Error();
        Log.Error("{Message}: {ErrorCode}", message, error);
        throw new Win32Exception(error);
    }

    private void RunMessageLoop()
    {
        MSG msg;
        while (GetMessage(out msg, HWND.Null, 0, 0) > 0)
        {
            TranslateMessage(in msg);
            DispatchMessage(in msg);
        }

        Log.Information("Цикл обработки сообщений ClipboardMonitor завершён.");
    }

    private LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == WmClipboardUpdate && !_token.IsCancellationRequested)
        {
            string text = _inputSimulator.GetClipboardText();

            if (string.IsNullOrWhiteSpace(text))
            {
                Log.Information("Буфер обмена пустой или не содержит текст.");
                return (LRESULT)0;
            }
            
            _ = ClipboardUpdate?.Invoke(text, _inputSimulator);

        }

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    protected override void DisposeManaged()
    {
        _tokenRegistration.Dispose();
        _messageLoopThread?.Join();

        Log.Information("WindowsClipboardMonitor.DisposeManaged вызван.");
    }

    protected override void DisposeUnmanaged()
    {
        if (!_hwnd.IsNull)
        {
            RemoveClipboardFormatListener(_hwnd);
            DestroyWindow(_hwnd);
        }

        UnregisterClass(_className, GetModuleHandle((PCWSTR)null));

        Log.Information("WindowsClipboardMonitor.DisposeUnmanaged вызван.");
    }
}
