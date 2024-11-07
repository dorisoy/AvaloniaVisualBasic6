using System;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class AddProcedureViewModel : ObservableObject, IDialog
{
    public string Title => "Add Procedure";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    [Notify] private string name = "";
    [Notify] private bool isSub = true;
    [Notify] private bool isProperty;
    [Notify] private bool isFunction;
    [Notify] private bool isEvent;
    [Notify] private bool isPublic = true;
    [Notify] private bool isPrivate;
    [Notify] private bool allLocalStatics;

    public DelegateCommand OkCommand { get; }

    public AddProcedureViewModel()
    {
        OkCommand = new DelegateCommand(() => CloseRequested?.Invoke(true), () => !string.IsNullOrWhiteSpace(name));
    }

    private void OnNameChanged() => OkCommand.RaiseCanExecutedChanged();

    public void Cancel()
    {
        CloseRequested?.Invoke(false);
    }

    public (string beginCode, string endCode) GenerateCode()
    {
        if (isEvent)
        {
            return ($"Public Event {name}()", "");
        }

        var visibility = isPublic ? "Public" : "Private";
        if (allLocalStatics)
            visibility += " Static";

        if (isProperty)
        {
            return ($"{visibility} Property Get {name}() As Variant\n", $"\nEnd Property\n\n{visibility} Property Let {name}(ByVal vNewValue As Variant)\n\nEnd Property\n");
        }

        var type = isSub ? "Sub" : "Function";
        return ($"{visibility} {type} {name}()\n", $"\nEnd {type}\n");
    }

}