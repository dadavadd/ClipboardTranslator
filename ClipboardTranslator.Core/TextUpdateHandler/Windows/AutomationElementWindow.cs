using Serilog;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.Foundation;

using static Windows.Win32.PInvoke;
using ClipboardTranslator.Core.Interfaces;
using ClipboardTranslator.Core.Configuration;
namespace ClipboardTranslator.Core.TextUpdateHandler.Windows;

internal sealed unsafe class AutomationElementWindow(IInputSimulator simulator,
                                                     TranslatorConfig config,
                                                     CancellationToken token) : WindowsMessageWindow(token), ITextUpdater
{

    private readonly VIRTUAL_KEY[] _keysToListen = KeyParser.ParseVirtualKeys(config.TranslationHotkey);
    public event Func<string, IInputSimulator, Task>? TextUpdate;

    protected override string WindowClassPrefix => "KeyListener";

    protected override WNDPROC WndProcDelegate => WndProc;

    private static LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == 0x0312 /* WM_HOTKEY */)
        {
            // TODO: написать реализацию для получения текста из под курсора.
            // Желательно использовать библиотеку Windows.UI.Automation.
            // Или может быть есть другой способ?
            Log.Information("Горячая клавиша: ID={0}", wParam.Value);
        }

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    protected override void Initialize()
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

    protected override void DisposeManaged()
    {
        Log.Information("AutomationElementWindow.DisposeManaged вызван");
        UnregisterHotKey(Hwnd, 0);
        base.DisposeManaged();
    }
}
