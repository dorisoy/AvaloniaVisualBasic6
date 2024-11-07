using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaVisualBasic.Runtime.Components;

namespace AvaloniaVisualBasic.VisualDesigner.Views;

public class PropertyConverter : IValueConverter
{
    public static PropertyConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is PropertyClass property &&
            value is ComponentInstanceViewModel instance)
            return instance.Instance.GetBoxedPropertyOrDefault(property);
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}