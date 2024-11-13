using Avalonia.Media;

namespace AvaloniaVisualBasic.Runtime.BuiltinTypes;

public readonly record struct VBFont
{
    public readonly FontFamily FontFamily;
    public readonly int Size;
    public readonly FontWeight Weight;
    public readonly FontStyle Style;

    public VBFont(FontFamily fontFamily, int size,
        FontWeight weight = FontWeight.Normal,
        FontStyle style = FontStyle.Normal)
    {
        FontFamily = fontFamily;
        Size = size;
        Weight = weight;
        Style = style;
    }

    public static VBFont Default { get; } = new VBFont(new FontFamily("fonts:App#MS Sans Serif"), 11);

    public override string ToString()
    {
        return FontFamily.ToString();
    }
}