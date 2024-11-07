using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Converters;

public class VBColorDescriptionConverter : IValueConverter
{
    public static VBColorDescriptionConverter Instance { get; } = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is VBColor vbColor)
        {
            if (vbColor.Type == VBColor.ColorType.SystemColor)
                return vbColor.SystemColor.GetDescription();
            return vbColor.ToString();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}