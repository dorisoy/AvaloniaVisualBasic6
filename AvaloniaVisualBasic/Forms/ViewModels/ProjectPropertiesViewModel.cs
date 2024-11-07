using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class ProjectPropertiesViewModel : ObservableObject, IDialog
{
    public string Title { get; }
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    public List<VBProjectType> ProjectTypes { get; } =
    [
        VBProjectType.EXE
    ];

    [Notify] private VBProjectType selectedProjectType;

    public List<ProjectStartupObjectViewModel> StartupObjects { get; }

    [Notify] private ProjectStartupObjectViewModel? selectedStartupObject;

    [Notify] private string projectName;

    [Notify] private string projectDescription;

    public DelegateCommand OkCommand { get; }

    public DelegateCommand CancelCommand { get; }

    public ProjectPropertiesViewModel(ProjectDefinition projectDefinition)
    {
        Title = $"{projectDefinition.Name} - Project Properties";
        selectedProjectType = projectDefinition.ProjectType;
        StartupObjects = projectDefinition.Forms.Select(x => new ProjectStartupObjectViewModel(x)).ToList();
        selectedStartupObject = StartupObjects.FirstOrDefault(x => x.Form == projectDefinition.StartupForm);
        projectName = projectDefinition.Name;
        projectDescription = projectDefinition.Description;

        OkCommand = new DelegateCommand(() => CloseRequested?.Invoke(true), () => !string.IsNullOrEmpty(projectName));
        CancelCommand = new DelegateCommand(() => CloseRequested?.Invoke(false), () => true);
    }

    public void Apply(ProjectDefinition projectDefinition)
    {
        projectDefinition.Name = projectName;
        projectDefinition.Description = projectDescription;
        projectDefinition.ProjectType = SelectedProjectType;
        projectDefinition.StartupForm = selectedStartupObject?.Form;
    }

    private void OnProjectNameChanged()
    {
        OkCommand.RaiseCanExecutedChanged();
    }
}