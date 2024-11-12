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
}