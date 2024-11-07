using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Classic.Avalonia.Theme;

namespace AvaloniaVisualBasic.IDE;

public partial class DialogWindow : ClassicWindow
{
    public DialogWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IDialog dialog)
        {
            dialog.CloseRequested += CloseRequested;
        }
    }

    private void CloseRequested(bool dialogResult)
    {
        this.Close(dialogResult);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (DataContext is IDialog dialog)
        {
            dialog.CloseRequested -= CloseRequested;
        }
    }
}