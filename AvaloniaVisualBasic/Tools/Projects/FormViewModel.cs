using System;
using System.ComponentModel;
using System.IO;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Tools;

public partial class FormViewModel : ObservableObject, IDisposable, IProjectTreeElement
{
    public ProjectViewModel Project { get; }
    public FormDefinition FormDefinition => form;

    private readonly FormDefinition form;

    [Notify] private bool isExpanded;
    [Notify] private string name;
    [Notify] private string file;

    public FormViewModel(ProjectViewModel project, FormDefinition form)
    {
        Project = project;
        this.form = form;
        name = form.Name;
        file = form.AbsolutePath == null ? form.Name : Path.GetFileName(form.AbsolutePath);
        form.PropertyChanged += OnChanged;
    }

    private void OnChanged(object? sender, PropertyChangedEventArgs e)
    {
        Name = form.Name;
        File = form.AbsolutePath == null ? form.Name : Path.GetFileName(form.AbsolutePath);
    }

    public void Dispose()
    {
        form.PropertyChanged -= OnChanged;
    }
}