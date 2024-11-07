using System.ComponentModel;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Projects;

public interface IFocusedProjectUtil : INotifyPropertyChanged
{
    public ProjectDefinition? FocusedProject { get; }
    public ProjectDefinition? FocusedOrStartupProject { get; }
    public FormDefinition? FocusedForm { get; }
    public string FocusedComponentPosition { get; }
    public string FocusedComponentSize { get; }
}