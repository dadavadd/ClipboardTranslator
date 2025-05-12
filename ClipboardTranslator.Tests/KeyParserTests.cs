using ClipboardTranslator.Core.TextUpdateHandler.Windows;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace ClipboardTranslator.Tests;

public class KeyParserTests
{
    [Fact]
    public void ParseVirtualKeys_None_ReturnsEmptyArray()
    {
        var result = KeyParser.ParseVirtualKeys("None");

        Assert.Empty(result);
    }

    [Fact]
    public void ParseVirtualKeys_SingleKey_ReturnsCorrectVirtualKey()
    {
        var result = KeyParser.ParseVirtualKeys("A");

        Assert.Single(result);
        Assert.Equal(VIRTUAL_KEY.VK_A, result[0]);
    }

    [Fact]
    public void ParseVirtualKeys_TwoKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("CTRL+C");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_CONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_C, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_MoreThanTwoKeys_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            KeyParser.ParseVirtualKeys("CTRL+SHIFT+A"));

        Assert.Contains("Комбинация может содержать максимум 2 клавиши", exception.Message);
    }

    [Fact]
    public void ParseVirtualKeys_SpecialKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("ENTER");

        Assert.Single(result);
        Assert.Equal(VIRTUAL_KEY.VK_RETURN, result[0]);
    }

    [Fact]
    public void ParseVirtualKeys_FunctionKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("CTRL+F1");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_CONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_F1, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_SymbolKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("CTRL+;");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_CONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_OEM_1, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_MixedCaseKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("Ctrl+Delete");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_CONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_DELETE, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_SpacesInInput_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("Ctrl + Alt");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_CONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_MENU, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_SpecificDirectionalKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("LCTRL+UP");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_LCONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_UP, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_NumpadKeys_ReturnsCorrectVirtualKeys()
    {
        var result = KeyParser.ParseVirtualKeys("CTRL+NUMPAD1");

        Assert.Equal(2, result.Length);
        Assert.Equal(VIRTUAL_KEY.VK_CONTROL, result[0]);
        Assert.Equal(VIRTUAL_KEY.VK_NUMPAD1, result[1]);
    }

    [Fact]
    public void ParseVirtualKeys_InvalidKey_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            KeyParser.ParseVirtualKeys("CTRL+INVALID_KEY"));

        Assert.Contains("Неизвестный токен", exception.Message);
    }
}
