using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AvaloniaVisualBasic.IDE;

public interface IMdiWindowManager : INotifyPropertyChanged
{
    public IMdiWindow? ActiveWindow { get; set; }
    public ObservableCollection<IMdiWindow> Windows { get; }
    void OpenWindow(IMdiWindow window);
    void CloseWindow(IMdiWindow window);
}