using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Tools;

public partial class ProjectViewModel : ObservableObject, IDisposable, IProjectTreeElement
{
    private readonly ProjectToolViewModel parentVm;
    private readonly ProjectDefinition projectDefinition;

    public string Name => projectDefinition.Name;

    public ObservableCollection<IProjectTreeElement> Elements { get; } = new();

    [Notify] private bool isExpanded = true;

    [Notify] private string file;

    [Notify] private bool isStartupProject;

    private ProjectFormsViewModel forms;

    private Dictionary<FormDefinition, FormViewModel> formViewModels = new();

    public ProjectDefinition Definition => projectDefinition;

    public ProjectViewModel(ProjectToolViewModel parentVm,
        ProjectDefinition projectDefinition)
    {
        this.parentVm = parentVm;
        this.projectDefinition = projectDefinition;
        forms = new ProjectFormsViewModel(this);
        file = projectDefinition.AbsolutePath == null ? projectDefinition.Name : Path.GetFileName(projectDefinition.AbsolutePath);
        projectDefinition.FormAdded += OnFormAdded;
        projectDefinition.FormDeleted += OnFormDeleted;
        projectDefinition.PropertyChanged += OnChanged;

        parentVm.PropertyChanged += ToolPropertyChanged;

        foreach (var form in this.projectDefinition.Forms)
            OnFormAdded(this.projectDefinition, form);
    }

    private void OnChanged(object? sender, PropertyChangedEventArgs e)
    {
        File = projectDefinition.AbsolutePath == null ? projectDefinition.Name : Path.GetFileName(projectDefinition.AbsolutePath);
        OnPropertyChanged(nameof(Name));
    }

    private void OnFormAdded(ProjectDefinition _, FormDefinition form)
    {
        var vm = new FormViewModel(this, form);
        formViewModels[form] = vm;
        forms.Forms.Add(vm);

        if (parentVm.ToggleFolders)
        {
            if (!Elements.Contains(forms))
                Elements.Add(forms);
        }
        else
        {
            Elements.Add(vm);
        }
    }

    private void OnFormDeleted(ProjectDefinition _, FormDefinition form)
    {
        var vm = formViewModels[form];
        forms.Forms.Remove(vm);
        vm.Dispose();

        if (parentVm.ToggleFolders)
        {
            if (forms.Forms.Count == 0)
                Elements.Remove(forms);
        }
        else
        {
            Elements.Remove(vm);
        }
    }

    public void Dispose()
    {
        foreach (var form in forms.Forms)
            form.Dispose();
        projectDefinition.PropertyChanged -= OnChanged;
        projectDefinition.FormAdded -=  OnFormAdded;
        projectDefinition.FormDeleted -= OnFormDeleted;

        parentVm.PropertyChanged -= ToolPropertyChanged;
    }

    private void ToolPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProjectToolViewModel.ToggleFolders))
        {
            Elements.Clear();
            if (parentVm.ToggleFolders)
            {
                if (forms.Forms.Count > 0)
                    Elements.Add(forms);
            }
            else
            {
                foreach (var formVm in forms.Forms)
                    Elements.Add(formVm);
            }
        }
    }
}