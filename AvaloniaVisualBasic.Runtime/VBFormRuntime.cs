using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using Classic.Avalonia.Theme;

namespace AvaloniaVisualBasic.Runtime;

public class VBFormRuntime : ClassicWindow, IModuleExecutionRoot
{
    protected override Type StyleKeyOverride => typeof(ClassicWindow);

    private VBWindowContext windowContext;

    public VBWindowContext Context => windowContext;
    
    public VBFormRuntime()
    {
        windowContext = new VBWindowContext(new StandaloneStandardLib(this));
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        windowContext.ExecuteSub("Form_Load");
    }

    protected override void OnResized(WindowResizedEventArgs e)
    {
        base.OnResized(e);
        windowContext.ExecuteSub("Form_Resize");
    }

    public void ExecuteSub(string name)
    {
        windowContext.ExecuteSub(name);
    }
}