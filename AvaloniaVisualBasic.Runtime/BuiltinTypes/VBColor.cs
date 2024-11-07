using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Classic.CommonControls;

namespace AvaloniaVisualBasic.Runtime.BuiltinTypes;

public readonly struct VBColor : IEquatable<VBColor>
{
    public readonly ColorType Type;
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;

    public VbSystemColor SystemColor => (VbSystemColor)R;

    public bool Equals(VBColor other) => Type == other.Type && R == other.R && G == other.G && B == other.B;

    public override bool Equals(object? obj) => obj is VBColor other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)Type, R, G, B);

    public static bool operator ==(VBColor left, VBColor right) => left.Equals(right);

    public static bool operator !=(VBColor left, VBColor right) => !left.Equals(right);

    private VBColor(ColorType type, byte r, byte g, byte b)
    {
        Type = type;
        R = r;
        G = g;
        B = b;
    }

    public override string ToString()
    {
        return $"&H{(int)Type:X2}{B:X2}{G:X2}{R:X2}&";
    }

    public static bool TryParse(string str, out VBColor result)
    {
        result = default;
        if (!str.StartsWith("&h", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!int.TryParse(str[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var type) ||
            !Enum.IsDefined(typeof(ColorType), type) ||
            !byte.TryParse(str[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var blue) ||
            !byte.TryParse(str[6..8], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var green) ||
            !byte.TryParse(str[8..10], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var red))
            return false;

        result = new VBColor((ColorType)type, red, green, blue);
        return true;
    }

    public static VBColor FromColor(Color color)
    {
        return new VBColor(ColorType.Raw, color.R, color.G, color.B);
    }

    public static VBColor FromColor(byte r, byte g, byte b)
    {
        return new VBColor(ColorType.Raw, r, g, b);
    }

    public static VBColor FromSystemColor(VbSystemColor systemColor)
    {
        return new VBColor(ColorType.SystemColor, (byte)systemColor, 0, 0);
    }

    public enum ColorType
    {
        Raw = 0,
        SystemColor = 0x80
    }

    public static VBColor Black => FromColor(Colors.Black);
}

public enum VbSystemColor
{
    Scrollbar = 0,
    Desktop = 1,
    Activecaption = 2,
    Inactivecaption = 3,
    Menu = 4,
    Window = 5,
    Windowframe = 6,
    Menutext = 7,
    Windowtext = 8,
    Captiontext = 9,
    Activeborder = 10,
    Inactiveborder = 11,
    Appworkspace = 12,
    Highlight = 13,
    Highlighttext = 14,
    Btnface = 15,
    Btnshadow = 16,
    Graytext = 17,
    Btntext = 18,
    Inactivecaptiontext = 19,
    Btnhighlight = 20,
    _3DDKSHADOW = 21,
    _3DLIGHT = 22,
    Infotext = 23,
    Infobk = 24,

    // Not used in VB6
    // HOTLIGHT = 26,
    // GRADIENTACTIVECAPTION = 27,
    // GRADIENTINACTIVECAPTION = 28,
    // MENUHILIGHT = 29,
    // MENUBAR = 30
}

public static class VBColorExtensions
{
    public static IBrush ToBrush(this VBColor color)
    {
        if (color.Type == VBColor.ColorType.Raw)
            return new SolidColorBrush(new Color(0xFF, color.R, color.G, color.B));

        return (IBrush)(Application.Current!.FindResource(GetKey(color.SystemColor)) ?? Brushes.DeepPink);
    }

    public static SystemResourceKey GetKey(this VbSystemColor color)
    {
        return color switch
        {
            VbSystemColor.Scrollbar => SystemColors.ScrollBarBrushKey,
            VbSystemColor.Desktop => SystemColors.DesktopBrushKey,
            VbSystemColor.Activecaption => SystemColors.ActiveCaptionBrushKey,
            VbSystemColor.Inactivecaption => SystemColors.InactiveCaptionBrushKey,
            VbSystemColor.Menu => SystemColors.MenuBrushKey,
            VbSystemColor.Window => SystemColors.WindowBrushKey,
            VbSystemColor.Windowframe => SystemColors.WindowFrameBrushKey,
            VbSystemColor.Menutext => SystemColors.MenuTextBrushKey,
            VbSystemColor.Windowtext => SystemColors.WindowTextBrushKey,
            VbSystemColor.Captiontext => SystemColors.ActiveCaptionTextBrushKey,
            VbSystemColor.Activeborder => SystemColors.ActiveBorderBrushKey,
            VbSystemColor.Inactiveborder => SystemColors.InactiveBorderBrushKey,
            VbSystemColor.Appworkspace => SystemColors.AppWorkspaceBrushKey,
            VbSystemColor.Highlight => SystemColors.HighlightBrushKey,
            VbSystemColor.Highlighttext => SystemColors.HighlightTextBrushKey,
            VbSystemColor.Btnface => SystemColors.ControlBrushKey,
            VbSystemColor.Btnshadow => SystemColors.ControlDarkBrushKey,
            VbSystemColor.Graytext => SystemColors.GrayTextBrushKey,
            VbSystemColor.Btntext => SystemColors.ControlTextBrushKey,
            VbSystemColor.Inactivecaptiontext => SystemColors.InactiveCaptionTextBrushKey,
            VbSystemColor.Btnhighlight => SystemColors.ControlLightBrushKey,
            VbSystemColor._3DDKSHADOW => SystemColors.ControlDarkDarkBrushKey,
            VbSystemColor._3DLIGHT => SystemColors.ControlLightLightBrushKey,
            VbSystemColor.Infotext => SystemColors.InfoTextBrushKey,
            VbSystemColor.Infobk => SystemColors.InfoBrushKey,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }

    public static string GetDescription(this VbSystemColor color)
    {
        return color switch
        {
            VbSystemColor.Scrollbar => "Scroll Bars",
            VbSystemColor.Desktop => "Desktop",
            VbSystemColor.Activecaption => "Active Title Bar",
            VbSystemColor.Inactivecaption => "Inactive Title Bar",
            VbSystemColor.Menu => "Menu Bar",
            VbSystemColor.Window => "Window Background",
            VbSystemColor.Windowframe => "Window Frame",
            VbSystemColor.Menutext => "Menu Text",
            VbSystemColor.Windowtext => "Window Text",
            VbSystemColor.Captiontext => "Active Title Bar Text",
            VbSystemColor.Activeborder => "Active Border",
            VbSystemColor.Inactiveborder => "Inactive Border",
            VbSystemColor.Appworkspace => "Application Workspace",
            VbSystemColor.Highlight => "Highlight",
            VbSystemColor.Highlighttext => "Highlight Text",
            VbSystemColor.Btnface => "Button Face",
            VbSystemColor.Btnshadow => "Button Shadow",
            VbSystemColor.Graytext => "Disabled Text",
            VbSystemColor.Btntext => "Button Text",
            VbSystemColor.Inactivecaptiontext => "Inactive Title Bar Text",
            VbSystemColor.Btnhighlight => "Button Highlight",
            VbSystemColor._3DDKSHADOW => "Button Dark Shadow",
            VbSystemColor._3DLIGHT => "Button Light Shadow",
            VbSystemColor.Infotext => "ToolTip Text",
            VbSystemColor.Infobk => "ToolTip",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }
}
