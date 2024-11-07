using System;
using Avalonia;
using Avalonia.Labs.Input;
using Classic.Avalonia.Theme;

namespace AvaloniaVisualBasic;

public partial class MainWindow : ClassicWindow
{
    private bool once;
    public MainWindow()
    {
        InitializeComponent();

        // this is required to make commands work without focusing the MainView first
        CommandManager.SetCommandBindings(this, CommandManager.GetCommandBindings(MainView));
        CommandManager.InvalidateRequerySuggested();

#if DEBUG
        this.AttachDevTools();
#endif

        Activated += OnActivated;
    }

    private void OnActivated(object? sender, EventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
        if (once)
            return;
        once = true;
        MainView.WindowInitialized();
    }
}