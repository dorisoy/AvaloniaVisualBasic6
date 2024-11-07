using System;
using System.Collections.Generic;
using System.ComponentModel;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.IDE;

public interface IProjectManager : INotifyPropertyChanged
{
    public IReadOnlyList<ProjectDefinition> LoadedProjects { get; }
    public event Action<ProjectDefinition>? ProjectLoaded;
    public event Action<ProjectDefinition>? ProjectUnloaded;

    public ProjectDefinition? StartupProject { get; set; }

    public ProjectDefinition NewProject(IProjectTemplate projectTemplate, string name);
    void AddProject(ProjectDefinition project);
    void UnloadAllProjects();
    void UnloadProject(ProjectDefinition projectManager);
}