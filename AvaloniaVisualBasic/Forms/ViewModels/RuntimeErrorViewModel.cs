using System;
using AvaloniaVisualBasic.IDE;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public class RuntimeErrorViewModel : ObservableObject, IDialog
{
    public string Title => "Avalonia Visual Basic";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;
    public string ErrorText { get; }

    public RuntimeErrorViewModel(string errorText)
    {
        ErrorText = errorText;
    }

    public void Continue()
    {
        CloseRequested?.Invoke(false);
    }

    public void End()
    {
        CloseRequested?.Invoke(false);
    }

    public void Debug()
    {
        CloseRequested?.Invoke(false);
    }

    public void Help()
    {
        CloseRequested?.Invoke(false);
    }

    public bool CanContinue() => false;
    public bool CanEnd() => true;
    public bool CanDebug() => false;
    public bool CanHelp() => false;
}