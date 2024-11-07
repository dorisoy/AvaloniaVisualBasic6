using System.Linq;
using Avalonia.Data;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaVisualBasic.VisualDesigner;

public partial class ComponentInstanceViewModel : ObservableObject
{
    private readonly FormEditViewModel parentViewModel;
    private ComponentInstance instance;

    public ComponentInstance Instance => instance;

    public ComponentInstanceViewModel(FormEditViewModel parentViewModel, ComponentInstance instance)
    {
        this.parentViewModel = parentViewModel;
        this.instance = instance;
        this.instance.OnComponentPropertyChanged += InstanceOnOnComponentPropertyChanged;
        this.Instance.OnComponentPropertyChanging += InstancePropertyChanging;
    }

    private void InstancePropertyChanging(ComponentInstance _, PropertyClass propertyClass, object? oldvalue, object? newValue)
    {
        if (propertyClass == VBProperties.NameProperty)
        {
            if (string.IsNullOrEmpty(newValue as string))
                throw new DataValidationException("Name can't be empty");

            if (parentViewModel.AllComponents.Any(c => c.Name == (newValue as string)))
                throw new DataValidationException("Name must be unique in form");
        }
    }

    private void InstanceOnOnComponentPropertyChanged(ComponentInstance _, PropertyClass propertyClass)
    {
        if (propertyClass == VBProperties.LeftProperty)
            OnPropertyChanged(nameof(Left));
        else if (propertyClass == VBProperties.TopProperty)
            OnPropertyChanged(nameof(Top));
        else if (propertyClass == VBProperties.WidthProperty)
            OnPropertyChanged(nameof(Width));
        else if (propertyClass == VBProperties.HeightProperty)
            OnPropertyChanged(nameof(Height));
        else if (propertyClass == VBProperties.NameProperty)
            OnPropertyChanged(nameof(Name));
        else if (propertyClass == VBProperties.CaptionProperty)
            OnPropertyChanged(nameof(Caption));
        else if (propertyClass == VBProperties.BackColorProperty)
            OnPropertyChanged(nameof(BackColor));
        else if (propertyClass == VBProperties.ForeColorProperty)
            OnPropertyChanged(nameof(ForeColor));
    }

    public double Left
    {
        get => instance.GetPropertyOrDefault(VBProperties.LeftProperty);
        set => instance.SetProperty(VBProperties.LeftProperty, value);
    }

    public double Top
    {
        get => instance.GetPropertyOrDefault(VBProperties.TopProperty);
        set => instance.SetProperty(VBProperties.TopProperty, value);
    }

    public double Width
    {
        get => instance.GetPropertyOrDefault(VBProperties.WidthProperty);
        set => instance.SetProperty(VBProperties.WidthProperty, value);
    }

    public double Height
    {
        get => instance.GetPropertyOrDefault(VBProperties.HeightProperty);
        set => instance.SetProperty(VBProperties.HeightProperty, value);
    }

    public string Name => instance.GetPropertyOrDefault(VBProperties.NameProperty) ?? "";

    public string? Caption => instance.GetPropertyOrDefault(VBProperties.CaptionProperty);

    public IBrush BackColor => instance.GetPropertyOrDefault(VBProperties.BackColorProperty).ToBrush();

    public IBrush ForeColor => instance.GetPropertyOrDefault(VBProperties.ForeColorProperty).ToBrush();

    public string BaseClassName => instance.BaseClass.Name;

}