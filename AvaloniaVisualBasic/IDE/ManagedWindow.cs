using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using AvaloniaVisualBasic.Controls;

namespace AvaloniaVisualBasic.IDE;

public class ManagedWindow : MDIWindow
{
    protected override Type StyleKeyOverride => typeof(MDIWindow);
    public event Action? RequestClose;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var buttons = e.NameScope.Get<MDICaptionButtons>("PART_CaptionButtons");
    }
}