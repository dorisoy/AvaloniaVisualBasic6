using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBOptionButton : RadioButton
{
    public static readonly StyledProperty<VBAppearance> AppearanceProperty = AvaloniaProperty.Register<VBOptionButton, VBAppearance>(nameof(Appearance), VBAppearance._3D);

    public VBAppearance Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    public static readonly StyledProperty<VBCheckValue> ValueProperty = AvaloniaProperty.Register<VBOptionButton, VBCheckValue>(nameof(Value));

    public VBCheckValue Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override void OnClick()
    {
        var was = IsChecked;
        base.OnClick();
        if (IsChecked == true && was != true)
        {
            this.ExecuteSub(ComponentBaseClass.ClickEvent);
        }
    }

    static VBOptionButton()
    {
        AppearanceProperty.Changed.AddClassHandler<VBOptionButton>((checkBox, e) =>
        {
            if (checkBox.Appearance == VBAppearance._3D)
                checkBox.Theme = null;
            else
                checkBox.Theme = Application.Current?.FindResource("FlatVBOptionButton") as ControlTheme;
        });
        ValueProperty.Changed.AddClassHandler<VBOptionButton>((checkBox, e) =>
        {
            checkBox.IsThreeState = checkBox.Value == VBCheckValue.Grayscale;
            checkBox.IsChecked = checkBox.Value switch
            {
                VBCheckValue.Unchecked => false,
                VBCheckValue.Checked => true,
                VBCheckValue.Grayscale => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        });
    }
}