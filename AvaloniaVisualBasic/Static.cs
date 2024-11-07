using System;

namespace AvaloniaVisualBasic;

public class Static
{
    public static bool ForceSingleView => false;

    public static bool SupportsWindowing { get; } = OperatingSystem.IsWindows() ||
                                             OperatingSystem.IsLinux() ||
                                             OperatingSystem.IsMacOS();

    public static bool SingleView { get; set; }

    public static MainView MainView { get; set; }
}