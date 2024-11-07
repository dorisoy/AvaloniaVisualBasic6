using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.VisualDesigner;

namespace AvaloniaVisualBasic.Controls;

public class PropertyBox : TemplatedControl
{
    private Panel? panel;
    public static readonly StyledProperty<IDataTemplate?> GenericTemplateProperty = AvaloniaProperty.Register<PropertyBox, IDataTemplate?>(nameof(GenericTemplate));
    public static readonly StyledProperty<IDataTemplate?> ColorTemplateProperty = AvaloniaProperty.Register<PropertyBox, IDataTemplate?>(nameof(ColorTemplate));
    public static readonly StyledProperty<IDataTemplate?> EnumTemplateProperty = AvaloniaProperty.Register<PropertyBox, IDataTemplate?>(nameof(EnumTemplate));
    public static readonly StyledProperty<IDataTemplate?> FontTemplateProperty = AvaloniaProperty.Register<PropertyBox, IDataTemplate?>(nameof(FontTemplate));
    public static readonly StyledProperty<IDataTemplate?> StringListTemplateProperty = AvaloniaProperty.Register<PropertyBox, IDataTemplate?>(nameof(StringListTemplate));

    public static readonly StyledProperty<PropertyClass?> PropertyClassProperty = AvaloniaProperty.Register<PropertyBox, PropertyClass?>("Property");
    public static readonly StyledProperty<object?> ObjectProperty = AvaloniaProperty.Register<PropertyBox, object?>("Object", defaultBindingMode: BindingMode.TwoWay);

    public object? Object
    {
        get => GetValue(ObjectProperty);
        set => SetValue(ObjectProperty, value);
    }

    public PropertyClass? PropertyClass
    {
        get => GetValue(PropertyClassProperty);
        set => SetValue(PropertyClassProperty, value);
    }

    public IDataTemplate? GenericTemplate
    {
        get => GetValue(GenericTemplateProperty);
        set => SetValue(GenericTemplateProperty, value);
    }

    public IDataTemplate? ColorTemplate
    {
        get => GetValue(ColorTemplateProperty);
        set => SetValue(ColorTemplateProperty, value);
    }

    public IDataTemplate? EnumTemplate
    {
        get => GetValue(EnumTemplateProperty);
        set => SetValue(EnumTemplateProperty, value);
    }

    public IDataTemplate? FontTemplate
    {
        get => GetValue(FontTemplateProperty);
        set => SetValue(FontTemplateProperty, value);
    }

    public IDataTemplate? StringListTemplate
    {
        get => GetValue(StringListTemplateProperty);
        set => SetValue(StringListTemplateProperty, value);
    }

    static PropertyBox()
    {
        PropertyClassProperty.Changed.AddClassHandler<PropertyBox>((box, e) =>
        {
            box.UpdatePropertyVisibility();
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        panel = e.NameScope.Get<Panel>("PART_Panel");
        UpdatePropertyVisibility();
    }

    private void UpdatePropertyVisibility()
    {
        if (panel == null)
            return;

        panel.Children.Clear();

        if (PropertyClass == null)
            return;

        if (PropertyClass.PropertyType == typeof(VBColor))
        {
            panel.Children.Add(ColorTemplate?.Build(null)!);
        }
        else if (PropertyClass.PropertyType == typeof(VBFont))
        {
            panel.Children.Add(FontTemplate?.Build(null)!);
        }
        else if (PropertyClass.PropertyType.IsEnum || PropertyClass.PropertyType == typeof(bool))
        {
            panel.Children.Add(EnumTemplate?.Build(null)!);
        }
        else if (PropertyClass.PropertyType == typeof(List<string>))
        {
            panel.Children.Add(StringListTemplate?.Build(null)!);
        }
        else
        {
            panel.Children.Add(GenericTemplate?.Build(null)!);
        }
    }
}