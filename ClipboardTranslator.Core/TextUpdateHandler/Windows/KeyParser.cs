using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace ClipboardTranslator.Core.TextUpdateHandler.Windows;

public class KeyParser
{
    private static readonly Dictionary<string, VIRTUAL_KEY> KeyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CTRL"] = VIRTUAL_KEY.VK_CONTROL,        // Generic Control
        ["SHIFT"] = VIRTUAL_KEY.VK_SHIFT,         // Generic Shift
        ["ALT"] = VIRTUAL_KEY.VK_MENU,            // Generic Alt (Menu)
        ["WIN"] = VIRTUAL_KEY.VK_LWIN,            // Generic Windows Key (defaults to Left)
        ["ENTER"] = VIRTUAL_KEY.VK_RETURN,
        ["ESC"] = VIRTUAL_KEY.VK_ESCAPE,
        ["SPACE"] = VIRTUAL_KEY.VK_SPACE,
        ["TAB"] = VIRTUAL_KEY.VK_TAB,
        ["UP"] = VIRTUAL_KEY.VK_UP,
        ["DOWN"] = VIRTUAL_KEY.VK_DOWN,
        ["LEFT"] = VIRTUAL_KEY.VK_LEFT,
        ["RIGHT"] = VIRTUAL_KEY.VK_RIGHT,

        ["A"] = VIRTUAL_KEY.VK_A,
        ["B"] = VIRTUAL_KEY.VK_B,
        ["C"] = VIRTUAL_KEY.VK_C,
        ["D"] = VIRTUAL_KEY.VK_D,
        ["E"] = VIRTUAL_KEY.VK_E,
        ["F"] = VIRTUAL_KEY.VK_F,
        ["G"] = VIRTUAL_KEY.VK_G,
        ["H"] = VIRTUAL_KEY.VK_H,
        ["I"] = VIRTUAL_KEY.VK_I,
        ["J"] = VIRTUAL_KEY.VK_J,
        ["K"] = VIRTUAL_KEY.VK_K,
        ["L"] = VIRTUAL_KEY.VK_L,
        ["M"] = VIRTUAL_KEY.VK_M,
        ["N"] = VIRTUAL_KEY.VK_N,
        ["O"] = VIRTUAL_KEY.VK_O,
        ["P"] = VIRTUAL_KEY.VK_P,
        ["Q"] = VIRTUAL_KEY.VK_Q,
        ["R"] = VIRTUAL_KEY.VK_R,
        ["S"] = VIRTUAL_KEY.VK_S,
        ["T"] = VIRTUAL_KEY.VK_T,
        ["U"] = VIRTUAL_KEY.VK_U,
        ["V"] = VIRTUAL_KEY.VK_V,
        ["W"] = VIRTUAL_KEY.VK_W,
        ["X"] = VIRTUAL_KEY.VK_X,
        ["Y"] = VIRTUAL_KEY.VK_Y,
        ["Z"] = VIRTUAL_KEY.VK_Z,

        ["0"] = VIRTUAL_KEY.VK_0,
        ["1"] = VIRTUAL_KEY.VK_1,
        ["2"] = VIRTUAL_KEY.VK_2,
        ["3"] = VIRTUAL_KEY.VK_3,
        ["4"] = VIRTUAL_KEY.VK_4,
        ["5"] = VIRTUAL_KEY.VK_5,
        ["6"] = VIRTUAL_KEY.VK_6,
        ["7"] = VIRTUAL_KEY.VK_7,
        ["8"] = VIRTUAL_KEY.VK_8,
        ["9"] = VIRTUAL_KEY.VK_9,

        ["F1"] = VIRTUAL_KEY.VK_F1,
        ["F2"] = VIRTUAL_KEY.VK_F2,
        ["F3"] = VIRTUAL_KEY.VK_F3,
        ["F4"] = VIRTUAL_KEY.VK_F4,
        ["F5"] = VIRTUAL_KEY.VK_F5,
        ["F6"] = VIRTUAL_KEY.VK_F6,
        ["F7"] = VIRTUAL_KEY.VK_F7,
        ["F8"] = VIRTUAL_KEY.VK_F8,
        ["F9"] = VIRTUAL_KEY.VK_F9,
        ["F10"] = VIRTUAL_KEY.VK_F10,
        ["F11"] = VIRTUAL_KEY.VK_F11,
        ["F12"] = VIRTUAL_KEY.VK_F12,
        ["F13"] = VIRTUAL_KEY.VK_F13,
        ["F14"] = VIRTUAL_KEY.VK_F14,
        ["F15"] = VIRTUAL_KEY.VK_F15,
        ["F16"] = VIRTUAL_KEY.VK_F16,
        ["F17"] = VIRTUAL_KEY.VK_F17,
        ["F18"] = VIRTUAL_KEY.VK_F18,
        ["F19"] = VIRTUAL_KEY.VK_F19,
        ["F20"] = VIRTUAL_KEY.VK_F20,
        ["F21"] = VIRTUAL_KEY.VK_F21,
        ["F22"] = VIRTUAL_KEY.VK_F22,
        ["F23"] = VIRTUAL_KEY.VK_F23,
        ["F24"] = VIRTUAL_KEY.VK_F24,

        ["PAGEUP"] = VIRTUAL_KEY.VK_PRIOR,        // Page Up
        ["PAGEDOWN"] = VIRTUAL_KEY.VK_NEXT,       // Page Down
        ["END"] = VIRTUAL_KEY.VK_END,
        ["HOME"] = VIRTUAL_KEY.VK_HOME,
        ["INSERT"] = VIRTUAL_KEY.VK_INSERT,
        ["DELETE"] = VIRTUAL_KEY.VK_DELETE,

        ["NUMPAD0"] = VIRTUAL_KEY.VK_NUMPAD0,
        ["NUMPAD1"] = VIRTUAL_KEY.VK_NUMPAD1,
        ["NUMPAD2"] = VIRTUAL_KEY.VK_NUMPAD2,
        ["NUMPAD3"] = VIRTUAL_KEY.VK_NUMPAD3,
        ["NUMPAD4"] = VIRTUAL_KEY.VK_NUMPAD4,
        ["NUMPAD5"] = VIRTUAL_KEY.VK_NUMPAD5,
        ["NUMPAD6"] = VIRTUAL_KEY.VK_NUMPAD6,
        ["NUMPAD7"] = VIRTUAL_KEY.VK_NUMPAD7,
        ["NUMPAD8"] = VIRTUAL_KEY.VK_NUMPAD8,
        ["NUMPAD9"] = VIRTUAL_KEY.VK_NUMPAD9,
        ["MULTIPLY"] = VIRTUAL_KEY.VK_MULTIPLY,   // Numpad *
        ["ADD"] = VIRTUAL_KEY.VK_ADD,             // Numpad +
        ["SEPARATOR"] = VIRTUAL_KEY.VK_SEPARATOR, // Numpad Separator (locale-dependent)
        ["SUBTRACT"] = VIRTUAL_KEY.VK_SUBTRACT,   // Numpad -
        ["DECIMAL"] = VIRTUAL_KEY.VK_DECIMAL,     // Numpad .
        ["DIVIDE"] = VIRTUAL_KEY.VK_DIVIDE,       // Numpad /
        ["NUMLOCK"] = VIRTUAL_KEY.VK_NUMLOCK,

        ["LSHIFT"] = VIRTUAL_KEY.VK_LSHIFT,
        ["RSHIFT"] = VIRTUAL_KEY.VK_RSHIFT,
        ["LCTRL"] = VIRTUAL_KEY.VK_LCONTROL,
        ["RCTRL"] = VIRTUAL_KEY.VK_RCONTROL,
        ["LALT"] = VIRTUAL_KEY.VK_LMENU,          // Left Alt
        ["RALT"] = VIRTUAL_KEY.VK_RMENU,          // Right Alt
        ["LWIN"] = VIRTUAL_KEY.VK_LWIN,           // Left Windows Key
        ["RWIN"] = VIRTUAL_KEY.VK_RWIN,           // Right Windows Key
        ["APPS"] = VIRTUAL_KEY.VK_APPS,           // Application Key (Context Menu)

        ["BROWSER_BACK"] = VIRTUAL_KEY.VK_BROWSER_BACK,
        ["BROWSER_FORWARD"] = VIRTUAL_KEY.VK_BROWSER_FORWARD,
        ["BROWSER_REFRESH"] = VIRTUAL_KEY.VK_BROWSER_REFRESH,
        ["BROWSER_STOP"] = VIRTUAL_KEY.VK_BROWSER_STOP,
        ["BROWSER_SEARCH"] = VIRTUAL_KEY.VK_BROWSER_SEARCH,
        ["BROWSER_FAVORITES"] = VIRTUAL_KEY.VK_BROWSER_FAVORITES,
        ["BROWSER_HOME"] = VIRTUAL_KEY.VK_BROWSER_HOME,
        ["VOLUME_MUTE"] = VIRTUAL_KEY.VK_VOLUME_MUTE,
        ["VOLUME_DOWN"] = VIRTUAL_KEY.VK_VOLUME_DOWN,
        ["VOLUME_UP"] = VIRTUAL_KEY.VK_VOLUME_UP,
        ["MEDIA_NEXT_TRACK"] = VIRTUAL_KEY.VK_MEDIA_NEXT_TRACK,
        ["MEDIA_PREV_TRACK"] = VIRTUAL_KEY.VK_MEDIA_PREV_TRACK,
        ["MEDIA_STOP"] = VIRTUAL_KEY.VK_MEDIA_STOP,
        ["MEDIA_PLAY_PAUSE"] = VIRTUAL_KEY.VK_MEDIA_PLAY_PAUSE,
        ["LAUNCH_MAIL"] = VIRTUAL_KEY.VK_LAUNCH_MAIL,
        ["LAUNCH_MEDIA_SELECT"] = VIRTUAL_KEY.VK_LAUNCH_MEDIA_SELECT,
        ["LAUNCH_APP1"] = VIRTUAL_KEY.VK_LAUNCH_APP1,
        ["LAUNCH_APP2"] = VIRTUAL_KEY.VK_LAUNCH_APP2,

        ["OEM_1"] = VIRTUAL_KEY.VK_OEM_1,           // ';:' for US
        ["OEM_PLUS"] = VIRTUAL_KEY.VK_OEM_PLUS,     // '=+' for US
        ["OEM_COMMA"] = VIRTUAL_KEY.VK_OEM_COMMA,   // ',<' for US
        ["OEM_MINUS"] = VIRTUAL_KEY.VK_OEM_MINUS,   // '-_' for US
        ["OEM_PERIOD"] = VIRTUAL_KEY.VK_OEM_PERIOD, // '.>' for US
        ["OEM_2"] = VIRTUAL_KEY.VK_OEM_2,           // '/?' for US
        ["OEM_3"] = VIRTUAL_KEY.VK_OEM_3,           // '`~' for US
        ["OEM_4"] = VIRTUAL_KEY.VK_OEM_4,           // '[{' for US
        ["OEM_5"] = VIRTUAL_KEY.VK_OEM_5,           // '\|' for US
        ["OEM_6"] = VIRTUAL_KEY.VK_OEM_6,           // ']}' for US
        ["OEM_7"] = VIRTUAL_KEY.VK_OEM_7,           // ''"' for US
        ["OEM_8"] = VIRTUAL_KEY.VK_OEM_8,           // Miscellaneous characters; varies by keyboard
        ["OEM_102"] = VIRTUAL_KEY.VK_OEM_102,       // Either the angle bracket key or the backslash key on the RT 102-key keyboard
        ["OEM_AX"] = VIRTUAL_KEY.VK_OEM_AX,         // 'AX' key on Japanese AX kbd
        ["OEM_CLEAR"] = VIRTUAL_KEY.VK_OEM_CLEAR,

        [";"] = VIRTUAL_KEY.VK_OEM_1,
        ["="] = VIRTUAL_KEY.VK_OEM_PLUS,
        [","] = VIRTUAL_KEY.VK_OEM_COMMA,
        ["-"] = VIRTUAL_KEY.VK_OEM_MINUS,
        ["."] = VIRTUAL_KEY.VK_OEM_PERIOD,
        ["/"] = VIRTUAL_KEY.VK_OEM_2,
        ["`"] = VIRTUAL_KEY.VK_OEM_3,
        ["["] = VIRTUAL_KEY.VK_OEM_4,
        ["\\"] = VIRTUAL_KEY.VK_OEM_5,
        ["]"] = VIRTUAL_KEY.VK_OEM_6,
        ["'"] = VIRTUAL_KEY.VK_OEM_7,

        ["BACKSPACE"] = VIRTUAL_KEY.VK_BACK,        // Backspace key
        ["CAPSLOCK"] = VIRTUAL_KEY.VK_CAPITAL,      // Caps Lock key
        ["SCROLLLOCK"] = VIRTUAL_KEY.VK_SCROLL,     // Scroll Lock key
        ["PRINTSCREEN"] = VIRTUAL_KEY.VK_SNAPSHOT,  // Print Screen key
        ["PAUSE"] = VIRTUAL_KEY.VK_PAUSE,           // Pause key
        ["SLEEP"] = VIRTUAL_KEY.VK_SLEEP,           // Computer Sleep key
        ["CLEAR"] = VIRTUAL_KEY.VK_CLEAR,           // Clear key (often Numpad 5 with Num Lock off)
        ["SELECT"] = VIRTUAL_KEY.VK_SELECT,
        ["PRINT"] = VIRTUAL_KEY.VK_PRINT,
        ["EXECUTE"] = VIRTUAL_KEY.VK_EXECUTE,
        ["HELP"] = VIRTUAL_KEY.VK_HELP,

        ["KANA"] = VIRTUAL_KEY.VK_KANA,             // Kana mode on Japanese keyboards (also HANGUL, HANGEUL)
        ["IME_ON"] = VIRTUAL_KEY.VK_IME_ON,
        ["JUNJA"] = VIRTUAL_KEY.VK_JUNJA,
        ["FINAL"] = VIRTUAL_KEY.VK_FINAL,
        ["HANJA"] = VIRTUAL_KEY.VK_HANJA,           // Hanja mode on Korean keyboards (also KANJI)
        ["IME_OFF"] = VIRTUAL_KEY.VK_IME_OFF,
        ["CONVERT"] = VIRTUAL_KEY.VK_CONVERT,       // IME Convert key
        ["NONCONVERT"] = VIRTUAL_KEY.VK_NONCONVERT,   // IME Nonconvert key
        ["ACCEPT"] = VIRTUAL_KEY.VK_ACCEPT,         // IME Accept key
        ["MODECHANGE"] = VIRTUAL_KEY.VK_MODECHANGE, // IME Mode change request
        ["PROCESSKEY"] = VIRTUAL_KEY.VK_PROCESSKEY, // Process key

        ["LBUTTON"] = VIRTUAL_KEY.VK_LBUTTON,     // Left mouse button
        ["RBUTTON"] = VIRTUAL_KEY.VK_RBUTTON,     // Right mouse button
        ["CANCEL"] = VIRTUAL_KEY.VK_CANCEL,       // Control-break processing
        ["MBUTTON"] = VIRTUAL_KEY.VK_MBUTTON,     // Middle mouse button
        ["XBUTTON1"] = VIRTUAL_KEY.VK_XBUTTON1,   // X1 mouse button
        ["XBUTTON2"] = VIRTUAL_KEY.VK_XBUTTON2,   // X2 mouse button

        ["PACKET"] = VIRTUAL_KEY.VK_PACKET,       // Used to pass Unicode characters as if they were keystrokes
        ["ATTN"] = VIRTUAL_KEY.VK_ATTN,           // Attn key
        ["CRSEL"] = VIRTUAL_KEY.VK_CRSEL,         // CrSel key
        ["EXSEL"] = VIRTUAL_KEY.VK_EXSEL,         // ExSel key
        ["EREOF"] = VIRTUAL_KEY.VK_EREOF,         // Erase EOF key
        ["PLAY"] = VIRTUAL_KEY.VK_PLAY,           // Play key
        ["ZOOM"] = VIRTUAL_KEY.VK_ZOOM,           // Zoom key
        ["NONAME"] = VIRTUAL_KEY.VK_NONAME,       // Reserved
        ["PA1"] = VIRTUAL_KEY.VK_PA1,             // PA1 key

        ["GAMEPAD_A"] = VIRTUAL_KEY.VK_GAMEPAD_A,
        ["GAMEPAD_B"] = VIRTUAL_KEY.VK_GAMEPAD_B,
        ["GAMEPAD_X"] = VIRTUAL_KEY.VK_GAMEPAD_X,
        ["GAMEPAD_Y"] = VIRTUAL_KEY.VK_GAMEPAD_Y,
        ["GAMEPAD_RIGHT_SHOULDER"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_SHOULDER,
        ["GAMEPAD_LEFT_SHOULDER"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_SHOULDER,
        ["GAMEPAD_LEFT_TRIGGER"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_TRIGGER,
        ["GAMEPAD_RIGHT_TRIGGER"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_TRIGGER,
        ["GAMEPAD_DPAD_UP"] = VIRTUAL_KEY.VK_GAMEPAD_DPAD_UP,
        ["GAMEPAD_DPAD_DOWN"] = VIRTUAL_KEY.VK_GAMEPAD_DPAD_DOWN,
        ["GAMEPAD_DPAD_LEFT"] = VIRTUAL_KEY.VK_GAMEPAD_DPAD_LEFT,
        ["GAMEPAD_DPAD_RIGHT"] = VIRTUAL_KEY.VK_GAMEPAD_DPAD_RIGHT,
        ["GAMEPAD_MENU"] = VIRTUAL_KEY.VK_GAMEPAD_MENU,
        ["GAMEPAD_VIEW"] = VIRTUAL_KEY.VK_GAMEPAD_VIEW,
        ["GAMEPAD_LEFT_THUMBSTICK_BUTTON"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_THUMBSTICK_BUTTON,
        ["GAMEPAD_RIGHT_THUMBSTICK_BUTTON"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_THUMBSTICK_BUTTON,
        ["GAMEPAD_LEFT_THUMBSTICK_UP"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_THUMBSTICK_UP,
        ["GAMEPAD_LEFT_THUMBSTICK_DOWN"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_THUMBSTICK_DOWN,
        ["GAMEPAD_LEFT_THUMBSTICK_RIGHT"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_THUMBSTICK_RIGHT,
        ["GAMEPAD_LEFT_THUMBSTICK_LEFT"] = VIRTUAL_KEY.VK_GAMEPAD_LEFT_THUMBSTICK_LEFT,
        ["GAMEPAD_RIGHT_THUMBSTICK_UP"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_THUMBSTICK_UP,
        ["GAMEPAD_RIGHT_THUMBSTICK_DOWN"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_THUMBSTICK_DOWN,
        ["GAMEPAD_RIGHT_THUMBSTICK_RIGHT"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_THUMBSTICK_RIGHT,
        ["GAMEPAD_RIGHT_THUMBSTICK_LEFT"] = VIRTUAL_KEY.VK_GAMEPAD_RIGHT_THUMBSTICK_LEFT,

        ["NAV_VIEW"] = VIRTUAL_KEY.VK_NAVIGATION_VIEW,
        ["NAV_MENU"] = VIRTUAL_KEY.VK_NAVIGATION_MENU,
        ["NAV_UP"] = VIRTUAL_KEY.VK_NAVIGATION_UP,
        ["NAV_DOWN"] = VIRTUAL_KEY.VK_NAVIGATION_DOWN,
        ["NAV_LEFT"] = VIRTUAL_KEY.VK_NAVIGATION_LEFT,
        ["NAV_RIGHT"] = VIRTUAL_KEY.VK_NAVIGATION_RIGHT,
        ["NAV_ACCEPT"] = VIRTUAL_KEY.VK_NAVIGATION_ACCEPT,
        ["NAV_CANCEL"] = VIRTUAL_KEY.VK_NAVIGATION_CANCEL,

        ["ABNT_C1"] = VIRTUAL_KEY.VK_ABNT_C1,
        ["ABNT_C2"] = VIRTUAL_KEY.VK_ABNT_C2,

        ["ICO_HELP"] = VIRTUAL_KEY.VK_ICO_HELP,
        ["ICO_00"] = VIRTUAL_KEY.VK_ICO_00,
        ["ICO_CLEAR"] = VIRTUAL_KEY.VK_ICO_CLEAR,

        ["OEM_FJ_JISHO"] = VIRTUAL_KEY.VK_OEM_FJ_JISHO,
        ["OEM_FJ_MASSHOU"] = VIRTUAL_KEY.VK_OEM_FJ_MASSHOU,
        ["OEM_FJ_TOUROKU"] = VIRTUAL_KEY.VK_OEM_FJ_TOUROKU,
        ["OEM_FJ_LOYA"] = VIRTUAL_KEY.VK_OEM_FJ_LOYA,
        ["OEM_FJ_ROYA"] = VIRTUAL_KEY.VK_OEM_FJ_ROYA,
        ["OEM_RESET"] = VIRTUAL_KEY.VK_OEM_RESET,
        ["OEM_JUMP"] = VIRTUAL_KEY.VK_OEM_JUMP,
        ["OEM_PA1"] = VIRTUAL_KEY.VK_OEM_PA1,
        ["OEM_PA2"] = VIRTUAL_KEY.VK_OEM_PA2,
        ["OEM_PA3"] = VIRTUAL_KEY.VK_OEM_PA3,
        ["OEM_WSCTRL"] = VIRTUAL_KEY.VK_OEM_WSCTRL,
        ["OEM_CUSEL"] = VIRTUAL_KEY.VK_OEM_CUSEL,
        ["OEM_ATTN"] = VIRTUAL_KEY.VK_OEM_ATTN,
        ["OEM_FINISH"] = VIRTUAL_KEY.VK_OEM_FINISH,
        ["OEM_COPY"] = VIRTUAL_KEY.VK_OEM_COPY,
        ["OEM_AUTO"] = VIRTUAL_KEY.VK_OEM_AUTO,
        ["OEM_ENLW"] = VIRTUAL_KEY.VK_OEM_ENLW,
        ["OEM_BACKTAB"] = VIRTUAL_KEY.VK_OEM_BACKTAB,

        ["DBE_ALPHANUMERIC"] = VIRTUAL_KEY.VK_DBE_ALPHANUMERIC,
        ["DBE_KATAKANA"] = VIRTUAL_KEY.VK_DBE_KATAKANA,
        ["DBE_HIRAGANA"] = VIRTUAL_KEY.VK_DBE_HIRAGANA,
        ["DBE_SBCSCHAR"] = VIRTUAL_KEY.VK_DBE_SBCSCHAR,
        ["DBE_DBCSCHAR"] = VIRTUAL_KEY.VK_DBE_DBCSCHAR,
        ["DBE_ROMAN"] = VIRTUAL_KEY.VK_DBE_ROMAN,
        ["DBE_NOROMAN"] = VIRTUAL_KEY.VK_DBE_NOROMAN,
        ["DBE_ENTERWORDREGISTERMODE"] = VIRTUAL_KEY.VK_DBE_ENTERWORDREGISTERMODE,
        ["DBE_ENTERIMECONFIGMODE"] = VIRTUAL_KEY.VK_DBE_ENTERIMECONFIGMODE,
        ["DBE_FLUSHSTRING"] = VIRTUAL_KEY.VK_DBE_FLUSHSTRING,
        ["DBE_CODEINPUT"] = VIRTUAL_KEY.VK_DBE_CODEINPUT,
        ["DBE_NOCODEINPUT"] = VIRTUAL_KEY.VK_DBE_NOCODEINPUT,
        ["DBE_DETERMINESTRING"] = VIRTUAL_KEY.VK_DBE_DETERMINESTRING,
        ["DBE_ENTERDLGCONVERSIONMODE"] = VIRTUAL_KEY.VK_DBE_ENTERDLGCONVERSIONMODE,
    };

    internal static VIRTUAL_KEY[] ParseVirtualKeys(string str)
    {
        var keys = str == "None" ? [] : str.Split('+', StringSplitOptions.RemoveEmptyEntries)
                  .Select(ParseSingleKey)
                  .ToArray();

        if (keys.Length > 2)
            throw new ArgumentException("Комбинация может содержать максимум 2 клавиши (например, CTRL+B). Указано: " + str);


        return keys;
    }

    private static VIRTUAL_KEY ParseSingleKey(string token)
    {
        token = token.Trim().ToUpperInvariant();

        if (KeyMap.TryGetValue(token, out var vk))
            return vk;

        if (token.Length == 1 && token[0] is >= 'A' and <= 'Z')
            return (VIRTUAL_KEY)('A' + (token[0] - 'A'));

        if (token.Length == 1 && token[0] is >= '0' and <= '9')
            return (VIRTUAL_KEY)('0' + (token[0] - '0'));

        throw new ArgumentException($"Неизвестный токен: {token}");
    }
}
