using Dock.Model.Mvvm.Controls;

namespace AvaloniaVisualBasic.Tools;

public class LocalsToolViewModel : Tool
{
    public LocalsToolViewModel()
    {
        Title = "Locals";
        CanPin = false;
        CanClose = true;
    }
}