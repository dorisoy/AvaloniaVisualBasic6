using System;
using Avalonia.Media;

namespace AvaloniaVisualBasic.IDE;

public interface IMdiWindow
{
    string Title { get; }
    IImage Icon { get; }
    event Action<IMdiWindow>? CloseRequest;
}
