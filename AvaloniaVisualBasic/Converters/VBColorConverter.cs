using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Converters;

public class VBColorConverter : IValueConverter
{
    public static VBColorConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is VBColor color)
        {
            if (targetType == typeof(string))
                return color.ToString();
            if (targetType == typeof(IBrush))
                return color.ToBrush();
            throw new NotSupportedException($"The target type {targetType} is not supported.");
        }
        if (value is string s)
        {
            if (VBColor.TryParse(s, out var color2))
                return color2;
            throw new FormatException($"Unable to convert \"{s}\" into {typeof(VBColor)}");
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}