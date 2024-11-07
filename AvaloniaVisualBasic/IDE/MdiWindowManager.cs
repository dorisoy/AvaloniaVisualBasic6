using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.IDE;

public partial class MdiWindowManager : ObservableObject, IMdiWindowManager
{
    [Notify] private IMdiWindow? activeWindow;
    public ObservableCollection<IMdiWindow> Windows { get; } = new();

    public void OpenWindow(IMdiWindow window)
    {
        Windows.Add(window);
        ActiveWindow = window;
        window.CloseRequest += WindowCloseRequest;
    }

    private void WindowCloseRequest(IMdiWindow window) => CloseWindow(window);

    public void CloseWindow(IMdiWindow window)
    {
        window.CloseRequest -= WindowCloseRequest;
        Windows.Remove(window);
        if (window is IDisposable disposable)
            disposable.Dispose();
    }
}

