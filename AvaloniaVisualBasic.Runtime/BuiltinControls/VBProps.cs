using Avalonia;
using Avalonia.Controls;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBProps
{
    public static readonly AttachedProperty<string?> NameProperty = AvaloniaProperty.RegisterAttached<VBProps, Control, string?>("Name");
    public static string? GetName(AvaloniaObject element) => element.GetValue(NameProperty);
    public static void SetName(AvaloniaObject element, string? value) => element.SetValue(NameProperty, value);
}