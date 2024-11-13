using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class FormDeserializer
{
    private static ComponentBaseClass[] AllSupportedComponents =
    [
        CheckBoxComponentClass.Instance,
        ComboBoxComponentClass.Instance,
        CommandButtonComponentClass.Instance,
        FormComponentClass.Instance,
        FrameComponentClass.Instance,
        HScrollBarComponentClass.Instance,
        LabelComponentClass.Instance,
        ListBoxComponentClass.Instance,
        MenuComponentClass.Instance,
        OptionButtonComponentClass.Instance,
        PictureBoxComponentClass.Instance,
        ShapeComponentClass.Instance,
        TextBoxComponentClass.Instance,
        TimerComponentClass.Instance,
        VScrollBarComponentClass.Instance,
    ];

    private static Dictionary<string, ComponentBaseClass> componentsByTypeNames;

    static FormDeserializer()
    {
        componentsByTypeNames = new();
        foreach (var component in AllSupportedComponents)
        {
            componentsByTypeNames[component.VBTypeName] = component;
        }
    }

    public FormDefinition? Deserialize(ProjectDefinition owner, string source, IDeserializeErrorSink errorSink)
    {
        var vb = new VbFrmFormatDeserializer();
        var (rootComponent, code) = vb.Deserialize(source);

        if (rootComponent.Type != FormComponentClass.Instance.VBTypeName)
        {
            errorSink.LogError($"This is not a valid form file");
            return null;
        }

        var form = new FormDefinition(owner, "");
        var components = new List<ComponentInstance>();

        void LoadRecur(VBSerializedComponent serializedComponent)
        {
            if (!componentsByTypeNames.TryGetValue(serializedComponent.Type, out var componentClass))
            {
                errorSink.LogError($"Class {serializedComponent.Type} of control {serializedComponent.Name} was not a loaded control class.");
                return;
            }

            var instance = new ComponentInstance(componentClass, serializedComponent.Name);
            components.Add(instance);

            foreach (var serializedProperty in serializedComponent.Properties)
            {
                var propertyName = serializedProperty.Key;
                if (propertyName == "ClientTop")
                    propertyName = "Top";
                if (propertyName == "ClientLeft")
                    propertyName = "Left";
                if (propertyName == "ClientWidth")
                    propertyName = "Width";
                if (propertyName == "ClientHeight")
                    propertyName = "Height";
                if (propertyName == "ScaleHeight" || propertyName == "ScaleWidth")
                    continue;
                if (!componentClass.PropertiesByName.TryGetValue(propertyName, out var propertyClass))
                {
                    errorSink.LogError($"The property name {serializedProperty.Key} in {serializedComponent.Name} is invalid.");
                    continue;
                }

                var val = serializedProperty.Value;
                void InvalidValue() => errorSink.LogError($"Property {serializedProperty.Key} in {serializedComponent.Name} had an invalid value.");
                if (propertyClass.PropertyType == typeof(string))
                {
                    if (val is not string)
                    {
                        InvalidValue();
                        continue;
                    }
                    instance.SetUntypedProperty(propertyClass, val);
                }
                else if (propertyClass.PropertyType == typeof(bool))
                {
                    if (val is not int i)
                    {
                        InvalidValue();
                        continue;
                    }
                    instance.SetUntypedProperty(propertyClass, i == -1);
                }
                else if (propertyClass.PropertyType == typeof(int))
                {
                    if (val is not int)
                    {
                        InvalidValue();
                        continue;
                    }
                    instance.SetUntypedProperty(propertyClass, val);
                }
                else if (propertyClass.PropertyType == typeof(float))
                {
                    if (val is int i)
                        instance.SetUntypedProperty(propertyClass, (float)i);
                    else if (val is float f)
                        instance.SetUntypedProperty(propertyClass, (float)f);
                    else
                        InvalidValue();
                }
                else if (propertyClass.PropertyType == typeof(double))
                {
                    var multiply = propertyClass == VBProperties.LeftProperty ||
                                   propertyClass == VBProperties.TopProperty ||
                                   propertyClass == VBProperties.WidthProperty ||
                                   propertyClass == VBProperties.HeightProperty
                        ? 1.0 / VBScaleModeExtensions.PixelToTwips
                        : 1;
                    if (val is int i)
                        instance.SetUntypedProperty(propertyClass, multiply * (double)i);
                    else if (val is float f)
                        instance.SetUntypedProperty(propertyClass, multiply * (double)f);
                    else if (val is double d)
                        instance.SetUntypedProperty(propertyClass, multiply * (double)d);
                    else
                        InvalidValue();
                }
                else if (propertyClass.PropertyType.IsEnum)
                {
                    if (val is int i)
                        instance.SetUntypedProperty(propertyClass, Enum.ToObject(propertyClass.PropertyType, i));
                    else
                        InvalidValue();
                }
                else if (propertyClass.PropertyType == typeof(VBColor))
                {
                    if (val is not VBColor color)
                    {
                        InvalidValue();
                        continue;
                    }
                    instance.SetUntypedProperty(propertyClass, color);
                }
                else if (propertyClass.PropertyType == typeof(VBFont))
                {
                    if (val is not Dictionary<string, object> fontProps ||
                        !fontProps.TryGetValue("Name", out var fontName) ||
                        !fontProps.TryGetValue("Size", out var fontSize) ||
                        !fontProps.TryGetValue("Weight", out var fontWeight) ||
                        !fontProps.TryGetValue("Italic", out var italic))
                    {
                        InvalidValue();
                        continue;
                    }

                    var font = new VBFont(
                        new FontFamily((string)fontName == "MS Sans Serif"? $"fonts:App#{fontName}" : (string)fontName),
                        (int)fontSize,
                        (FontWeight)fontWeight,
                        (int)italic != 0 ? FontStyle.Italic : FontStyle.Normal);
                    instance.SetUntypedProperty(propertyClass, font);
                }
            }

            foreach (var nested in serializedComponent.SubComponents)
                LoadRecur(nested);
        }

        LoadRecur(rootComponent);

        form.UpdateCode(code);
        form.UpdateComponents(components);
        return form;
    }
}