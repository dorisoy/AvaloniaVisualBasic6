using System;
using Avalonia.Controls;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBComboBox : ComboBox
{
    protected override Type StyleKeyOverride => typeof(ComboBox);
}