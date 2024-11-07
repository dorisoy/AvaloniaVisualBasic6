using System.Collections.ObjectModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Tools;

public partial class ProjectFormsViewModel : IProjectTreeElement
{
    public ProjectViewModel Project { get; }

    [Notify] private bool isExpanded = true;

    public ProjectFormsViewModel(ProjectViewModel project)
    {
        Project = project;
    }

    public ObservableCollection<FormViewModel> Forms { get; } = new();
}