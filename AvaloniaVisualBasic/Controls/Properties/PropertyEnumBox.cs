using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;

namespace AvaloniaVisualBasic.Controls;

public class PropertyEnumBox : TemplatedControl
{
    private ComboBox? comboBox;
    private bool syncing;

    public static readonly StyledProperty<object?> SelectedValueProperty = AvaloniaProperty.Register<PropertyEnumBox, object?>("SelectedValue");
    public static readonly StyledProperty<PropertyEnumViewModel?> SelectedViewModelProperty = AvaloniaProperty.Register<PropertyEnumBox, PropertyEnumViewModel?>("SelectedViewModel");
    public static readonly StyledProperty<Type?> PropertyTypeProperty = AvaloniaProperty.Register<PropertyEnumBox, Type?>("PropertyType");

    public static readonly StyledProperty<List<PropertyEnumViewModel>?> OptionsProperty = AvaloniaProperty.Register<PropertyEnumBox, List<PropertyEnumViewModel>?>("Options");

    public Type? PropertyType
    {
        get => GetValue(PropertyTypeProperty);
        set => SetValue(PropertyTypeProperty, value);
    }

    public List<PropertyEnumViewModel>? Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public PropertyEnumViewModel? SelectedViewModel
    {
        get => GetValue(SelectedViewModelProperty);
        set => SetValue(SelectedViewModelProperty, value);
    }

    public object? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    static PropertyEnumBox()
    {
        PropertyTypeProperty.Changed.AddClassHandler<PropertyEnumBox>((box, e) =>
        {
            box.UpdateOptions();
        });
        SelectedValueProperty.Changed.AddClassHandler<PropertyEnumBox>((box, e) =>
        {
            if (box.syncing)
                return;
            box.syncing = true;
            if (box.Options != null && box.PropertyType != null && box.SelectedValue != null)
            {
                if (box.PropertyType.IsEnum)
                {
                    var selectedUnderlyingValue = Convert.ChangeType(box.SelectedValue, box.PropertyType.GetEnumUnderlyingType());
                    box.SetCurrentValue(SelectedViewModelProperty, box.Options.FirstOrDefault(opt => Equals(opt.UnderlyingValue, selectedUnderlyingValue)));
                }
                else if (box.PropertyType == typeof(bool))
                {
                    box.SetCurrentValue(SelectedViewModelProperty, box.Options.FirstOrDefault(opt => Equals(opt.UnderlyingValue, box.SelectedValue)));
                }
                else
                    throw new Exception("this should never happen!");
            }
            box.syncing = false;
        });
        SelectedViewModelProperty.Changed.AddClassHandler<PropertyEnumBox>((box, e) =>
        {
            if (box.syncing)
                return;
            box.syncing = true;
            if (box.SelectedViewModel == null || box.PropertyType == null)
                box.SetCurrentValue(SelectedValueProperty, null);
            else if (box.PropertyType.IsEnum)
               box.SetCurrentValue(SelectedValueProperty, Enum.ToObject(box.PropertyType, box.SelectedViewModel.UnderlyingValue));
            else if (box.PropertyType == typeof(bool))
                box.SetCurrentValue(SelectedValueProperty, box.SelectedViewModel.UnderlyingValue);
            else
                throw new Exception($"This should never happen.");
            box.syncing = false;
        });
    }

    private void UpdateOptions()
    {
        if (PropertyType != null && PropertyType.IsEnum)
        {
            var newOptions = new List<PropertyEnumViewModel>();
            foreach (var enumValue in Enum.GetValuesAsUnderlyingType(PropertyType).OfType<object>()
                         .Zip(Enum.GetNames(PropertyType)))
            {
                newOptions.Add(new PropertyEnumViewModel(enumValue.First, $"{enumValue.First} - {enumValue.Second.TrimStart('_')}"));
            }
            SetCurrentValue(OptionsProperty, newOptions);
            if (SelectedValue != null)
            {
                var selectedUnderlyingValue = Convert.ChangeType(SelectedValue, PropertyType.GetEnumUnderlyingType());
                SetCurrentValue(SelectedViewModelProperty, newOptions.FirstOrDefault(opt => Equals(opt.UnderlyingValue, selectedUnderlyingValue)));
            }
            else
            {
                SetCurrentValue(SelectedViewModelProperty, null);
            }
        }
        else if (PropertyType == typeof(bool))
        {
            var newOptions = new List<PropertyEnumViewModel>();
            newOptions.Add(new PropertyEnumViewModel(false, "False"));
            newOptions.Add(new PropertyEnumViewModel(true, "True"));
            SetCurrentValue(OptionsProperty, newOptions);
            if (SelectedValue is bool b)
            {
                SetCurrentValue(SelectedViewModelProperty, newOptions.FirstOrDefault(opt => Equals(opt.UnderlyingValue, b)));
            }
            else
            {
                SetCurrentValue(SelectedViewModelProperty, null);
            }
        }
        else
        {
            SetCurrentValue(OptionsProperty, []);
            SetCurrentValue(SelectedViewModelProperty, null);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        comboBox = e.NameScope.Get<ComboBox>("PART_ComboBox");
    }
}

public class PropertyEnumViewModel
{
    public object UnderlyingValue { get; }
    private string toString;

    public PropertyEnumViewModel(object underlyingValue,
        string name)
    {
        UnderlyingValue = underlyingValue;
        toString = name;
    }

    public override string ToString() => toString;
}