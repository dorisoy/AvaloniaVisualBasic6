using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBScrollBar : ScrollBar
{
    protected override Type StyleKeyOverride => typeof(ScrollBar);

    public VBScrollBar()
    {
        ValueChanged += OnValueChanged;
    }

    private void OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        this.ExecuteSub(ScrollBarComponentClass.ChangeEvent);
    }

    public IReadOnlyList<PropertyClass> AccessibleProperties { get; } = [VBProperties.ValueProperty];

    public Vb6Value? GetPropertyValue(PropertyClass property)
    {
        if (property == VBProperties.ValueProperty)
            return new Vb6Value((int)Value);
        return null;
    }

    public void SetPropertyValue(PropertyClass property, Vb6Value value)
    {
        if (property == VBProperties.ValueProperty)
            Value = value.Value as int? ?? 0;
    }
}