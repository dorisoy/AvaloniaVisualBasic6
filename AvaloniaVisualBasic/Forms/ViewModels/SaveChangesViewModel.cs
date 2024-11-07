using System;
using System.Collections.ObjectModel;
using AvaloniaEdit.Utils;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public class SaveChangesViewModel : ObservableObject, IDialog
{
    public string Title => "Avalonia Visual Basic";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    public ObservableCollection<ChangedFileViewModel> ChangedFiles { get; } = new();

    public ObservableCollection<ChangedFileViewModel> SelectedFiles { get; } = new();

    public SaveChangesViewModel()
    {
    }

    public bool SaveChanges { get; private set; }

    public void Yes()
    {
        SaveChanges = true;
        CloseRequested?.Invoke(true);
    }

    public void No()
    {
        SaveChanges = false;
        CloseRequested?.Invoke(true);
    }

    public void Cancel()
    {
        CloseRequested?.Invoke(false);
    }

    public void Add(ProjectDefinition project)
    {
        ChangedFiles.Add(new ChangedFileViewModel(project));
    }

    public void Add(FormDefinition form)
    {
        ChangedFiles.Add(new ChangedFileViewModel(form));
    }
}

public class ChangedFileViewModel : ObservableObject
{
    public ProjectDefinition? Project { get; }
    public FormDefinition? Form { get; }

    public ChangedFileViewModel(ProjectDefinition project)
    {
        Project = project;
        Name = project.Name;
        Indent = false;
    }

    public ChangedFileViewModel(FormDefinition form)
    {
        Form = form;
        Name = form.Name;
        Indent = true;
    }

    public string Name { get; }

    public bool Indent { get; }

    public override string ToString()
    {
        return Indent ? $"    {Name}" : Name;
    }
}