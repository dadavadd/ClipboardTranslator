using System.Runtime.InteropServices;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.ComponentModel;
using ClipboardTranslator.Core.Interfaces;
using Windows.Win32.UI.Input.KeyboardAndMouse;

using static Windows.Win32.PInvoke;
using ClipboardTranslator.Core.Configuration;
using Serilog;

namespace ClipboardTranslator.Core.TextUpdateHandler.Windows;

public unsafe class WindowsClipboardMonitor(IInputSimulator inputSimulator,
                                            TranslatorConfig config,
                                            CancellationToken token) : WindowsMessageWindow(token), ITextUpdater
{
    private readonly IInputSimulator _inputSimulator = inputSimulator;
    private CancellationToken _token = token;
    private readonly VIRTUAL_KEY[] _keysToListen = KeyParser.ParseVirtualKeys(config.TranslationHotkey);

    private bool _isClipboardListenerMode = config.TranslationInputMode == "Clipboard" && config.TranslationHotkey == "None";

    public event Func<string, IInputSimulator, Task>? TextUpdate;

    protected override string WindowClassPrefix => "ClipboardMonitor";

    protected override WNDPROC WndProcDelegate => WndProc;

    protected override void Initialize()
    {
        if (_isClipboardListenerMode)
        {
            if (!AddClipboardFormatListener(Hwnd))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        else
        {
            HOT_KEY_MODIFIERS modifiers = 0;
            VIRTUAL_KEY mainKey = default;

            foreach (var key in _keysToListen)
            {
                switch (key)
                {
                    case VIRTUAL_KEY.VK_CONTROL:
                        modifiers |= (HOT_KEY_MODIFIERS)MOD_CONTROL;
                        break;
                    case VIRTUAL_KEY.VK_MENU: // ALT
                        modifiers |= (HOT_KEY_MODIFIERS)MOD_ALT;
                        break;
                    case VIRTUAL_KEY.VK_SHIFT:
                        modifiers |= (HOT_KEY_MODIFIERS)MOD_SHIFT;
                        break;
                    case VIRTUAL_KEY.VK_LWIN:
                    case VIRTUAL_KEY.VK_RWIN:
                        modifiers |= (HOT_KEY_MODIFIERS)MOD_WIN;
                        break;
                    default:
                        mainKey = key;
                        break;
                }
            }
            
            if (!RegisterHotKey(Hwnd, 0, modifiers, (uint)mainKey))
            {
                Log.Error("Не удалось зарегистрировать комбинацию клавиш: {Modifiers}+{Key}", modifiers, mainKey);
            }
        }
    }

    private LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (_isClipboardListenerMode)
        {
            if (msg == 0x031D /*WM_CLIPBOARDUPDATE*/ && !_token.IsCancellationRequested)
            {
                string text = _inputSimulator.GetClipboardText();
                if (!string.IsNullOrWhiteSpace(text))
                    _ = TextUpdate?.Invoke(text, _inputSimulator);
            }
        }
        else
        {
            if (msg == 0x0312 /* WM_HOTKEY */ && wParam == 0 && !_token.IsCancellationRequested)
            {
                string text = _inputSimulator.CopyAndGetClipboardText();
                if (!string.IsNullOrWhiteSpace(text))
                    _ = TextUpdate?.Invoke(text, _inputSimulator);
            }
        }

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    protected override void DisposeUnmanaged()
    {
        Log.Information("WindowsClipboardMonitor.DisposeUnmanaged вызван");
        RemoveClipboardFormatListener(Hwnd);
        base.DisposeUnmanaged();
    }
}
