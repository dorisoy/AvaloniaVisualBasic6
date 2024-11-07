using System.ComponentModel;

namespace AvaloniaVisualBasic.Tools;

public interface IProjectTreeElement : INotifyPropertyChanged
{
    public bool IsExpanded { get; set; }
}