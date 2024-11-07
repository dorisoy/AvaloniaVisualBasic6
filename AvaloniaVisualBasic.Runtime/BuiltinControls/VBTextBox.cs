using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);

    public IReadOnlyList<PropertyClass> AccessibleProperties { get; } = [VBProperties.TextProperty];

    static VBTextBox()
    {
        TextProperty.Changed.AddClassHandler<VBTextBox>((textBox, e) =>
        {
            textBox.ExecuteSub(TextBoxComponentClass.ChangeEvent);
        });
    }

    public Vb6Value? GetPropertyValue(PropertyClass property)
    {
        if (property == VBProperties.TextProperty)
            return new Vb6Value(Text ?? "");
        return null;
    }

    public void SetPropertyValue(PropertyClass property, Vb6Value value)
    {
        if (property == VBProperties.TextProperty)
        {
            Text = value.Value as string;
        }
    }

}