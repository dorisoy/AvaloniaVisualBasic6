namespace AvaloniaVisualBasic.Runtime.BuiltinTypes;

public enum VBScaleMode
{
    User,
    Twip,
    Point,
    Pixel,
    Character,
    Inch,
    Millimiter,
    Centimiter
}

public static class VBScaleModeExtensions
{
    public const int PixelToTwips = 15;
}