using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaVisualBasic.Controls;

public class PropertyStringListBox : TemplatedControl
{
    public static readonly StyledProperty<List<string>?> ElementsProperty = AvaloniaProperty.Register<PropertyStringListBox, List<string>?>("Elements");

    public static readonly StyledProperty<string?> ElementsAsTextProperty = AvaloniaProperty.Register<PropertyStringListBox, string?>("ElementsAsText");

    public List<string>? Elements
    {
        get => GetValue(ElementsProperty);
        set => SetValue(ElementsProperty, value);
    }

    public string? ElementsAsText
    {
        get => GetValue(ElementsAsTextProperty);
        set => SetValue(ElementsAsTextProperty, value);
    }

    private bool syncing = false;

    static PropertyStringListBox()
    {
        ElementsProperty.Changed.AddClassHandler<PropertyStringListBox>((box, e) =>
        {
            if (box.syncing)
                return;

            box.syncing = true;
            box.SetCurrentValue(ElementsAsTextProperty, string.Join("\n", box.Elements ?? []));
            box.syncing = false;
        });
        ElementsAsTextProperty.Changed.AddClassHandler<PropertyStringListBox>((box, e) =>
        {
            if (box.syncing)
                return;

            box.syncing = true;
            box.SetCurrentValue(ElementsProperty, (box.ElementsAsText ?? "").Split('\n').ToList());
            box.syncing = false;
        });
    }
}