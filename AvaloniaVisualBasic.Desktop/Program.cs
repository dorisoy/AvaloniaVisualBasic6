using System;
using System.IO;
using Avalonia;
using Avalonia.Media.Fonts;
using Classic.CommonControls;

namespace AvaloniaVisualBasic.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        FixCurrentWorkingDictionary();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void FixCurrentWorkingDictionary()
    {
        if (Path.GetDirectoryName(Environment.ProcessPath) is { } dir)
        {
            Environment.CurrentDirectory = dir;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseMessageBoxSounds()
            .LogToTrace()
            .ConfigureFonts(manager =>
            {
                manager.AddFontCollection(new EmbeddedFontCollection(new Uri("fonts:App", UriKind.Absolute),
                    new Uri("avares://AvaloniaVisualBasic/Resources", UriKind.Absolute)));
            });
}
