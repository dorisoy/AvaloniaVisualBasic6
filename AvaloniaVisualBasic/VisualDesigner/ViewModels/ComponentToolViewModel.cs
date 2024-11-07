using Avalonia.Media.Imaging;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaVisualBasic.VisualDesigner;

public partial class ComponentToolViewModel : ObservableObject
{
    private readonly ToolBoxToolViewModel parent;

    public ComponentToolViewModel(ToolBoxToolViewModel parent, ComponentBaseClass baseClass, Bitmap icon)
    {
        this.parent = parent;
        BaseClass = baseClass;
        Icon = icon;
        Name = baseClass.Name;
    }

    public ComponentToolViewModel(ToolBoxToolViewModel parent, string name, Bitmap icon)
    {
        this.parent = parent;
        Icon = icon;
        Name = name;
    }

    public ComponentBaseClass? BaseClass { get; }
    public Bitmap Icon { get; }
    public string Name { get; }

    public bool IsSelected
    {
        get => parent.SelectedComponent == this;
        set
        {
            if (value)
                parent.SelectedComponent = this;
            else
                parent.SelectedComponent = parent.Components[0];
        }
    }

    public void RaiseIsSelected() => OnPropertyChanged(nameof(IsSelected));
}