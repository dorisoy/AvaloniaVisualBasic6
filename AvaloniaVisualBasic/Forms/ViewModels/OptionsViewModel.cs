using System;
using AvaloniaVisualBasic.IDE;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public class OptionsViewModel : ObservableObject, IDialog
{
    public string Title => "Options";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    public void Close()
    {
        CloseRequested?.Invoke(false);
    }
}