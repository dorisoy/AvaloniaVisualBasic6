using System;
using Avalonia.Controls;
using Avalonia.Labs.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void AvaloniaOnWeb(object? sender, ExecutedRoutedEventArgs e)
    {
        TopLevel.GetTopLevel(this).Launcher.LaunchUriAsync(new Uri("https://avaloniaui.net"));
    }

    private void About(object? sender, ExecutedRoutedEventArgs e)
    {
        AboutDialog.ShowDialog(this.VisualRoot as Window, new AboutDialogOptions()
        {
            Copyright = "Copyleft BAndysc 2024",
            Title = "Avalonia Visual Basic 6",
            SubTitle = "For 32-bit and 64-bit cross-platform Development",
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaVisualBasic/Icons/about.gif")))
        });
    }

    public MainView WindowInitialized()
    {
        if (DataContext is MainViewViewModel vm)
        {
            vm.OnInitialized();
        }
        return this;
    }
}