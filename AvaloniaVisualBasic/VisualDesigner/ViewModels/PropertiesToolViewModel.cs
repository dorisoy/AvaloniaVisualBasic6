using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Utils;
using Classic.CommonControls.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using PropertyChanged.SourceGenerator;
using R3;

namespace AvaloniaVisualBasic.VisualDesigner;

public partial class PropertiesToolViewModel : Tool
{
    private readonly IWindowManager windowManager;
    public ObservableCollection<ComponentInstanceViewModel>? ComponentsProxy => currentDocument?.AllComponents;
    public ObservableCollection<PropertyViewModel> Properties { get; } = new();
    public ObservableCollection<BasePropertyViewModel> CategorizedProperties { get; } = new();
    [Notify] private PropertyViewModel selectedProperty;

    public ComponentInstanceViewModel? SelectedComponentProxy
    {
        get => currentDocument?.SelectedComponent;
        set
        {
            if (currentDocument != null)
                currentDocument.SelectedComponent = value;
        }
    }

    private System.IDisposable? currentDocumentSub;
    private FormEditViewModel? currentDocument;
    private ComponentInstanceViewModel? currentComponent;

    public PropertiesToolViewModel(IMdiWindowManager mdiWindowManager,
        IWindowManager windowManager)
    {
        this.windowManager = windowManager;
        Title = "Properties";
        CanPin = false;
        CanClose = true;

        mdiWindowManager
            .ObservePropertyChanged(x => x.ActiveWindow)
            .Subscribe(document =>
            {
                Properties.Clear();
                CategorizedProperties.Clear();
                currentDocumentSub?.Dispose();
                currentDocument = null;
                OnPropertyChanged(nameof(ComponentsProxy));
                if (currentComponent != null)
                {
                    currentComponent.Instance.OnComponentPropertyChanged -= OnComponentValueChanged;
                    currentComponent = null;
                }
                if (document is FormEditViewModel formEditViewModel)
                {
                    currentDocument = formEditViewModel;
                    OnPropertyChanged(nameof(ComponentsProxy));
                    currentDocumentSub = formEditViewModel.ObservePropertyChanged(y => y.SelectedComponent)
                        .Subscribe(component =>
                        {
                            Properties.Clear();
                            CategorizedProperties.Clear();

                            if (currentComponent != null)
                            {
                                currentComponent.Instance.OnComponentPropertyChanged -= OnComponentValueChanged;
                                currentComponent = null;
                            }

                            if (component != null)
                            {
                                currentComponent = component;
                                Dictionary<PropertyClass, PropertyViewModel> properties = new();
                                foreach (var propertyClass in component.Instance.BaseClass.Properties.OrderBy(prop => prop.Name))
                                {
                                    var prop = properties[propertyClass] = new PropertyViewModel(this, propertyClass, component.Instance.GetBoxedPropertyOrDefault(propertyClass));
                                    Properties.Add(prop);
                                }
                                foreach (var categoryGroup in component.Instance.BaseClass.Properties.GroupBy(p => p.Category))
                                {
                                    CategorizedProperties.Add(new PropertyCategoryViewModel(this, categoryGroup.Key));
                                    foreach (var propertyClass in categoryGroup.OrderBy(prop => prop.Name))
                                    {
                                        var prop = properties[propertyClass];
                                        CategorizedProperties.Add(prop);
                                    }
                                }

                                currentComponent.Instance.OnComponentPropertyChanged += OnComponentValueChanged;
                            }

                            OnPropertyChanged(nameof(SelectedComponentProxy));
                        });
                }
            });
    }

    private void OnComponentValueChanged(ComponentInstance instance, PropertyClass property)
    {
        foreach (var prop in Properties)
        {
            if (prop.PropertyClass == property)
            {
                prop.UpdateValueNoRaise(instance.GetBoxedPropertyOrDefault(property));
                break;
            }
        }
    }

    public void UpdateValue(PropertyClass propertyClass, object? value)
    {
        if (currentDocument == null)
            return;

        if (currentDocument.SelectedComponent == null)
            return;

        try
        {
            if (value is string stringValue)
            {
                if (propertyClass.TryParseString(stringValue, out var typedValue))
                    currentDocument.SelectedComponent.Instance.SetUntypedProperty(propertyClass, typedValue);
                else
                    throw new Exception();
            }
            else if (value == null)
            {
                currentDocument.SelectedComponent.Instance.SetUntypedProperty(propertyClass, null);
            }
            else if (value.GetType() == propertyClass.PropertyType)
            {
                currentDocument.SelectedComponent.Instance.SetUntypedProperty(propertyClass, value);
            }
            else
            {
                throw new Exception();
            }
        }
        catch (DataValidationException e)
        {
            windowManager.MessageBox(e.Message, icon: MessageBoxIcon.Error).ListenErrors();
        }
        catch (Exception e)
        {
            windowManager.MessageBox("Invalid property value", icon: MessageBoxIcon.Error).ListenErrors();
        }
    }
}

public abstract partial class BasePropertyViewModel : ObservableObject
{
    [Notify] private bool isVisible = true;
}

public partial class PropertyCategoryViewModel : BasePropertyViewModel
{
    private readonly PropertiesToolViewModel parent;
    private readonly PropertyCategory category;
    [Notify] private bool isExpanded = true;

    public PropertyCategoryViewModel(PropertiesToolViewModel parent, PropertyCategory category)
    {
        this.parent = parent;
        this.category = category;
        Header = category.ToString();
    }

    private void OnIsExpandedChanged()
    {
        foreach (var property in parent.Properties)
        {
            if (property.PropertyClass.Category == category)
                property.IsVisible = IsExpanded;
        }
    }

    public string Header { get; }
}

public partial class PropertyViewModel : BasePropertyViewModel
{
    private readonly PropertiesToolViewModel parent;

    public PropertyViewModel(PropertiesToolViewModel parent,
        PropertyClass propertyClass,
        object? value)
    {
        this.parent = parent;
        PropertyClass = propertyClass;
        Name = propertyClass.Name;
        this.value = value;
        Description = propertyClass.Description;
    }

    public void OnValueChanged()
    {
        parent.UpdateValue(PropertyClass, Value);
        OnPropertyChanged(nameof(Value));
    }

    public string Name { get; }
    [Notify] private object? value;
    public string Description { get; }
    public PropertyClass PropertyClass { get; }

    public void UpdateValueNoRaise(object? newValue)
    {
        this.value = newValue;
        OnPropertyChanged(nameof(Value));
    }
}