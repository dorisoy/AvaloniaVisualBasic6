using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Utils;
using Dock.Model.Mvvm.Controls;
using PropertyChanged.SourceGenerator;
using R3;

namespace AvaloniaVisualBasic.Tools;

public partial class ProjectToolViewModel : Tool
{
    private readonly IProjectManager projectManager;
    private readonly IEventBus eventBus;
    private readonly IProjectService projectService;
    private readonly IEditorService editorService;
    private Dictionary<ProjectDefinition, ProjectViewModel> projectToViewModel = new();

    public ObservableCollection<ProjectViewModel> LoadedProjects { get; } = new();

    [Notify] [AlsoNotify(nameof(SelectedForm), nameof(SelectedProject), nameof(SelectedForms), nameof(SelectedFormOrForms))] private object? selectedItem;

    [Notify] private bool toggleFolders = true;

    public FormViewModel? SelectedForm
    {
        get => selectedItem as FormViewModel;
        set => SelectedItem = value;
    }

    public ProjectFormsViewModel? SelectedForms
    {
        get => selectedItem as ProjectFormsViewModel;
        set => SelectedItem = value;
    }

    public ProjectViewModel? SelectedProject
    {
        get => selectedItem as ProjectViewModel;
        set => SelectedItem = value;
    }

    public bool SelectedFormOrForms => SelectedForm != null || SelectedForms != null;

    public DelegateCommand ViewObjectCommand { get; }

    public DelegateCommand ViewCodeCommand { get; }

    public DelegateCommand FormPropertiesCommand { get; }

    public DelegateCommand AddFormCommand { get; }

    private ProjectViewModel? GetSelectedProject()
    {
        if (SelectedProject != null)
            return SelectedProject;
        if (SelectedForms != null)
            return SelectedForms.Project;
        if (SelectedForm != null)
            return SelectedForm.Project;
        return null;
    }

    public ProjectToolViewModel(IProjectManager projectManager,
        IEventBus eventBus,
        IProjectService projectService,
        IEditorService editorService)
    {
        this.projectManager = projectManager;
        this.eventBus = eventBus;
        this.projectService = projectService;
        this.editorService = editorService;
        Title = "Project Group - Group1";
        CanPin = false;
        CanClose = true;

        ViewObjectCommand = new DelegateCommand(() =>
        {
            if (SelectedForm != null)
                editorService.EditForm(SelectedForm.FormDefinition);
        }, () => SelectedForm != null);
        ViewCodeCommand = new DelegateCommand(() =>
        {
            if (SelectedForm != null)
                editorService.EditCode(SelectedForm.FormDefinition);
        }, () => SelectedForm != null);
        FormPropertiesCommand = new DelegateCommand(() =>
        {
            ViewObjectCommand.Execute(null);
            ((ICommand)ApplicationCommands.OpenPropertiesCommand).Execute(null);
        }, () => SelectedForm != null);
        AddFormCommand = new DelegateCommand(() =>
        {
            var project = GetSelectedProject();
            project!.Definition.AddForm(new FormDefinition(project!.Definition, $"Form{project.Definition.Forms.Count + 1}"));
        }, () => GetSelectedProject() != null);

        projectManager.ProjectLoaded += OnProjectLoaded;
        projectManager.ProjectUnloaded += OnProjectUnloaded;
        foreach (var projDef in this.projectManager.LoadedProjects)
            OnProjectLoaded(projDef);

        projectManager.ObservePropertyChanged(x => x.StartupProject)
            .Subscribe(startupProj =>
            {
                foreach (var vm in LoadedProjects)
                    vm.IsStartupProject = vm.Definition == startupProj;
            });
    }

    private void OnSelectedItemChanged()
    {
        ViewObjectCommand.RaiseCanExecutedChanged();
        ViewCodeCommand.RaiseCanExecutedChanged();
        FormPropertiesCommand.RaiseCanExecutedChanged();
        AddFormCommand.RaiseCanExecutedChanged();
    }

    private void OnProjectLoaded(ProjectDefinition obj)
    {
        var vm = new ProjectViewModel(this, obj);
        projectToViewModel[obj] = vm;
        LoadedProjects.Add(vm);
    }

    private void OnProjectUnloaded(ProjectDefinition obj)
    {
        var vm = projectToViewModel[obj];
        LoadedProjects.Remove(vm);
        vm.Dispose();
        projectToViewModel.Remove(obj);
    }

    public void OpenSelected()
    {
        if (SelectedForm != null)
        {
            editorService.EditForm(SelectedForm.FormDefinition);
        }
        else if (selectedItem is IProjectTreeElement projectTreeElement)
        {
            projectTreeElement.IsExpanded = !projectTreeElement.IsExpanded;
        }
    }

    public void SetAsStartUp()
    {
        if (SelectedProject == null)
            return;

        projectManager.StartupProject = SelectedProject.Definition;
    }

    public void EditProjectProperties()
    {
        if (SelectedProject == null)
            return;

        projectService.EditProjectProperties(SelectedProject.Definition).ListenErrors();
    }

    public void DeleteForm()
    {
        if (SelectedForm == null)
            return;

        // TODO: this logic doesn't belong to a VM, maybe some service?
        eventBus.Publish(new FormUnloadedEvent(SelectedForm.FormDefinition));
        SelectedForm.Project.Definition.DeleteForm(SelectedForm.FormDefinition);
    }
}