using System;
using Avalonia.Controls;

namespace AvaloniaVisualBasic.Runtime.BuiltinTypes;

public enum VBStartupPosition
{
    StartUpManual = 0,
    StartUpOwner = 1,
    StartUpScreen = 2,
    StartUpWindowsDefault = 3,
}

public static class VBStartupPositionExtensions
{
    public static WindowStartupLocation ToAvalonia(this VBStartupPosition position)
    {
        return position switch
        {
            VBStartupPosition.StartUpManual => WindowStartupLocation.Manual,
            VBStartupPosition.StartUpOwner => WindowStartupLocation.CenterOwner,
            VBStartupPosition.StartUpScreen => WindowStartupLocation.CenterScreen,
            VBStartupPosition.StartUpWindowsDefault => WindowStartupLocation.CenterScreen,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };
    }
}