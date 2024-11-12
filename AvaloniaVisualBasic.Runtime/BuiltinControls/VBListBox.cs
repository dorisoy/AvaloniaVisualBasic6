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

    static VBListBox()
    {
        AttachedEvents.AttachClick<VBListBox>();
    }
}