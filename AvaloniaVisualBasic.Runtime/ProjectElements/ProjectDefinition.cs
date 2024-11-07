using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Runtime.ProjectElements;

public partial class ProjectDefinition : INotifyPropertyChanged
{
    [Notify] private FormDefinition? startupForm;

    [Notify] private string? absolutePath;

    [Notify] private string name;

    [Notify] private string description = "";

    [Notify] private VBProjectType projectType;

    private List<FormDefinition> forms = new();

    public ProjectDefinition(VBProjectType projectType, string name)
    {
        this.name = name;
        this.projectType = projectType;
    }

    public IReadOnlyList<FormDefinition> Forms => forms;

    public event Action<ProjectDefinition, FormDefinition>? FormAdded;

    public event Action<ProjectDefinition, FormDefinition>? FormDeleted;

    public void AddForm(FormDefinition form)
    {
        forms.Add(form);
        FormAdded?.Invoke(this, form);
        StartupForm ??= form;
    }

    public void DeleteForm(FormDefinition form)
    {
        forms.Remove(form);
        FormDeleted?.Invoke(this, form);
        if (startupForm == form)
        {
            startupForm = forms.FirstOrDefault();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}