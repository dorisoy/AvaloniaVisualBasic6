using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaVisualBasic.Runtime;
using Classic.Avalonia.Theme;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic.Standalone;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (Program.StartupForm != null)
            {
                VBLoader.RunForm(Program.StartupForm, default, out var window);
                desktop.MainWindow = window;
            }
            else
            {
                var window = new ClassicWindow()
                {
                    SizeToContent = SizeToContent.WidthAndHeight,
                    CanResize = false
                };
                var msgBox = new MessageBox()
                {
                    Text = Program.Error ?? "Unknown error",
                    Icon = MessageBoxIcon.Error,
                    Buttons = MessageBoxButtons.Ok
                };
                window.Content = msgBox;
                desktop.MainWindow = window;
                msgBox.AcceptRequest += _ => window.Close();
            }
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            throw new NotImplementedException();
        }

        base.OnFrameworkInitializationCompleted();
    }
}