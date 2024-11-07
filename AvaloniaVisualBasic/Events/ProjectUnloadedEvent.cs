using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Events;

public class ProjectUnloadedEvent : IEvent
{
    public ProjectDefinition Project { get; }

    public ProjectUnloadedEvent(ProjectDefinition project)
    {
        Project = project;
    }
}