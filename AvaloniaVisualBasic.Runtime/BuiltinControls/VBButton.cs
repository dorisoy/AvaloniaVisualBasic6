using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using AvaloniaVisualBasic.Runtime.Components;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBCommandButton : Button
{
    protected override Type StyleKeyOverride => typeof(Button);

    protected override void OnClick()
    {
        base.OnClick();
        this.ExecuteSub(ComponentBaseClass.ClickEvent);
    }
}