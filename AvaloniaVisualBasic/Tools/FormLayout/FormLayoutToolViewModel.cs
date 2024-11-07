using Dock.Model.Mvvm.Controls;

namespace AvaloniaVisualBasic.Tools;

public class FormLayoutToolViewModel : Tool
{
    public FormLayoutToolViewModel()
    {
        Title = "Form Layout";
        CanPin = false;
        CanClose = true;
    }
}