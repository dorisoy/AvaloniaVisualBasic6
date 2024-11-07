using System.Linq;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class FormSerializer
{
    public string Serialize(FormDefinition element)
    {
        VbFrmFormatSerializer vb = new VbFrmFormatSerializer();

        var form = element.Components.Single(x => x.BaseClass is FormComponentClass);

        vb.Begin(form.BaseClass.VBTypeName, form.GetPropertyOrDefault(VBProperties.NameProperty)!);

        WriteAllProperties(vb, form);
        WriteFormMeasurements(vb, form);

        foreach (var component in element.Components)
        {
            if (component != form)
            {
                vb.Begin(component.BaseClass.VBTypeName, component.GetPropertyOrDefault(VBProperties.NameProperty)!);
                WriteAllProperties(vb, component);
                vb.End();
            }
        }

        vb.End();

        vb.WriteCode(element.Code);

        return vb.GetOutput();
    }

    private void WriteFormMeasurements(VbFrmFormatSerializer vb, ComponentInstance form)
    {
        if (form.TryGetProperty(VBProperties.WidthProperty, out var width))
        {
            vb.WriteProperty("ClientWidth", VBProperties.WidthProperty.PropertyType, width * VBScaleModeExtensions.PixelToTwips);
            vb.WriteProperty("ScaleWidth", VBProperties.WidthProperty.PropertyType, width * VBScaleModeExtensions.PixelToTwips);
        }
        if (form.TryGetProperty(VBProperties.HeightProperty, out var height))
        {
            vb.WriteProperty("ClientHeight", VBProperties.HeightProperty.PropertyType, height * VBScaleModeExtensions.PixelToTwips);
            vb.WriteProperty("ScaleHeight", VBProperties.HeightProperty.PropertyType, height * VBScaleModeExtensions.PixelToTwips);
        }
        if (form.TryGetProperty(VBProperties.TopProperty, out var top))
        {
            vb.WriteProperty("ClientTop", VBProperties.TopProperty.PropertyType, top * VBScaleModeExtensions.PixelToTwips);
        }
        if (form.TryGetProperty(VBProperties.LeftProperty, out var left))
        {
            vb.WriteProperty("ClientLeft", VBProperties.LeftProperty.PropertyType, left * VBScaleModeExtensions.PixelToTwips);
        }
    }

    private void WriteAllProperties(VbFrmFormatSerializer vb, ComponentInstance instance)
    {
        foreach (var prop in instance.BaseClass.Properties)
        {
            if (prop == VBProperties.NameProperty)
                continue;

            if (prop == VBProperties.TopProperty ||
                prop == VBProperties.LeftProperty ||
                prop == VBProperties.WidthProperty ||
                prop == VBProperties.HeightProperty)
            {
                if (instance.TryGetProperty<double>((PropertyClass<double>)prop, out var measurement))
                    vb.WriteProperty(prop.Name, prop.PropertyType, measurement * VBScaleModeExtensions.PixelToTwips);
            }
            else
            {
                if (instance.TryGetBoxedProperty(prop, out var boxedValue))
                {
                    vb.WriteProperty(prop.Name, prop.PropertyType, boxedValue);
                }
            }
        }
    }
}