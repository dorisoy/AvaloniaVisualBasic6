using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace AvaloniaVisualBasic.Converters;

public class StringFormatConverter : IMultiValueConverter
{
    public string Format { get; set; } = "";

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Format(Format, values.ToArray());
    }
}