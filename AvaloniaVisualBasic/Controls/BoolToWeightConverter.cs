using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaVisualBasic.Controls;

public class BoolToWeightConverter : IValueConverter
{
    public FontWeight WhenTrue { get; set; } = FontWeight.Regular;
    public FontWeight WhenFalse { get; set; } = FontWeight.Regular;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? WhenTrue : WhenFalse;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}