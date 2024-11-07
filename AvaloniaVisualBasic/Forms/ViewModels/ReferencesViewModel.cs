using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class ReferencesViewModel : ObservableObject, IDialog
{
    public string Title { get; }
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    public DelegateCommand OkCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public List<ReferencesReferenceViewModel> References { get; } = new();
    [Notify] private ReferencesReferenceViewModel? selectedReference;

    public ReferencesViewModel(ProjectDefinition definition)
    {
        Title = $"References - {definition.Name}";

        References.Add(new ReferencesReferenceViewModel("Visual Basic For Applications", "built-in", "English/Standard"));
        selectedReference = References.FirstOrDefault();

        OkCommand = new DelegateCommand(() => CloseRequested?.Invoke(true));
        CancelCommand = new DelegateCommand(() => CloseRequested?.Invoke(false));
    }
}

public partial class ReferencesReferenceViewModel
{
    public ReferencesReferenceViewModel(string name, string location, string language)
    {
        Name = name;
        Location = location;
        Language = language;
    }

    public string Name { get; }
    public string Location { get; }
    public string Language { get; }
    [Notify] private bool isSelected;
}