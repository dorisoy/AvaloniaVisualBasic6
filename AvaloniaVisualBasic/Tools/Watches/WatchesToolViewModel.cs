using Dock.Model.Mvvm.Controls;

namespace AvaloniaVisualBasic.Tools;

public class WatchesToolViewModel : Tool
{
    public WatchesToolViewModel()
    {
        Title = "Watches";
        CanPin = false;
        CanClose = true;
    }
}