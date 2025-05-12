using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;

using static Windows.Win32.PInvoke;

namespace ClipboardTranslator.Core.TextUpdateHandler.Windows;

public abstract unsafe class WindowsMessageWindow : DisposableBase
{
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;

    private readonly PCWSTR _className;
    private HWND _hwnd;
    private Thread? _messageThread;
    private uint _messageThreadId;
    private CancellationTokenRegistration _tokenRegistration;

    protected HWND Hwnd => _hwnd;
    protected abstract string WindowClassPrefix { get; }
    protected abstract WNDPROC WndProcDelegate { get; }

    protected WindowsMessageWindow(CancellationToken token)
    {
        fixed (char* firstChar = (WindowClassPrefix + "_" + Guid.NewGuid()).ToCharArray())
            _className = firstChar;

        _messageThread = new(() =>
        {
            _messageThreadId = GetCurrentThreadId();
            _tokenRegistration = token.Register(() =>
                PostThreadMessage(_messageThreadId, 0x0012 /*WM_QUIT*/, 0, 0));

            RegisterWindowClass();
            CreateMessageWindow();
            Initialize();

            MSG msg;
            while (GetMessage(out msg, HWND.Null, 0, 0) > 0)
            {
                TranslateMessage(in msg);
                DispatchMessage(in msg);
            }
        });

        _messageThread.SetApartmentState(ApartmentState.STA);
        _messageThread.IsBackground = true;
        _messageThread.Start();
    }

    private void RegisterWindowClass()
    {
        var wcex = new WNDCLASSEXW
        {
            cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
            lpfnWndProc = WndProcDelegate,
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
                               0, 0, 0, 0,
                               new HWND(-3), HMENU.Null,
                               GetModuleHandle((PCWSTR)null), null);

        if (_hwnd.IsNull)
            ThrowLastWin32Exception("Не удалось создать окно");
    }

    protected virtual void Initialize() { }

    private static void ThrowLastWin32Exception(string message)
    {
        int error = Marshal.GetLastWin32Error();
        Log.Error("{Message}: {ErrorCode}", message, error);
        throw new Win32Exception(error);
    }

    protected override void DisposeManaged()
    {
        _tokenRegistration.Dispose();
        _messageThread?.Join();
    }

    protected override void DisposeUnmanaged()
    {
        if (!_hwnd.IsNull)
            DestroyWindow(_hwnd);

        UnregisterClass(_className, GetModuleHandle((PCWSTR)null));
    }
}
