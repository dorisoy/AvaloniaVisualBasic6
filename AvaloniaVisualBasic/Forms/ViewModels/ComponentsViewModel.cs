using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class ComponentsViewModel : ObservableObject, IDialog
{
    public string Title { get; }
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    public DelegateCommand OkCommand { get; }
    public DelegateCommand ApplyCommand { get; }
    public DelegateCommand CancelCommand { get; }

    [Notify] [AlsoNotify(nameof(Controls))] private bool showOnlySelectedControls;
    private List<ComponentsComponentViewModel> allControls { get; } = new();
    private List<ComponentsComponentViewModel> selectedControls { get; } = new();
    public List<ComponentsComponentViewModel> Controls => showOnlySelectedControls ? selectedControls : allControls;
    [Notify] private ComponentsComponentViewModel? selectedControl;

    [Notify] [AlsoNotify(nameof(Designers))] private bool showOnlySelectedDesigners;
    private List<ComponentsComponentViewModel> allDesigners { get; } = new();
    private List<ComponentsComponentViewModel> selectedDesigners { get; } = new();
    public List<ComponentsComponentViewModel> Designers => showOnlySelectedDesigners ? selectedDesigners : allDesigners;
    [Notify] private ComponentsComponentViewModel? selectedDesigner;

    [Notify] [AlsoNotify(nameof(InsertableObjects))] private bool showOnlySelectedInsertableObjects;
    private List<ComponentsComponentViewModel> allInsertableObjects { get; } = new();
    private List<ComponentsComponentViewModel> selectedInsertableObjects { get; } = new();
    public List<ComponentsComponentViewModel> InsertableObjects => showOnlySelectedInsertableObjects ? selectedInsertableObjects : allInsertableObjects;
    [Notify] private ComponentsComponentViewModel? selectedInsertableObject;

    public ComponentsViewModel(ProjectDefinition projectDefinition)
    {
        Title = $"Components";

        allControls.Add(new ComponentsComponentViewModel("Built-in controls", "built-in")
        {
            IsSelected = true
        });
        selectedControls.AddRange(allControls.Where(x => x.IsSelected));

        selectedControl = allControls.FirstOrDefault();
        selectedDesigner = allDesigners.FirstOrDefault();
        selectedInsertableObject = allInsertableObjects.FirstOrDefault();

        OkCommand = new DelegateCommand(() => CloseRequested?.Invoke(true));
        ApplyCommand = new DelegateCommand(() =>
        {

        });
        CancelCommand = new DelegateCommand(() => CloseRequested?.Invoke(false));
    }
}

public partial class ComponentsComponentViewModel : ObservableObject
{
    public ComponentsComponentViewModel(string name, string location)
    {
        Name = name;
        Location = location;
    }

    public string Name { get; }
    public string Location { get; }
    [Notify] private bool isSelected;
}
