using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBListBox : ListBox
{
    protected override Type StyleKeyOverride => typeof(ListBox);

    public IReadOnlyList<PropertyClass> AccessibleProperties { get; } = [VBProperties.ListIndexProperty];

    public Vb6Value? GetPropertyValue(PropertyClass property)
    {
        if (property == VBProperties.ListIndexProperty)
            return new Vb6Value(SelectedIndex);
        return null;
    }

    public void SetPropertyValue(PropertyClass property, Vb6Value value)
    {
        if (property == VBProperties.ListIndexProperty)
        {
            SelectedIndex = value.Value as int? ?? -1;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        this.ExecuteSub(ComponentBaseClass.ClickEvent);
    }
}