using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace AvaloniaVisualBasic.Converters;

public class StringMultiplierConverter : IValueConverter
{
    public string String { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
            return string.Join("", Enumerable.Repeat(String, intValue));
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}