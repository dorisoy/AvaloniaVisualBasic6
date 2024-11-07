using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;
using AvaloniaVisualBasic.Runtime.Utils;

namespace AvaloniaVisualBasic.Runtime.AvaloniaInterop;

public static class AvaloniaInteroperability
{
    private static List<IAvaloniaBinding> bindings = new();

    private static Dictionary<PropertyClass, List<IAvaloniaBinding>> bindingsByProperty = new();

    private interface IAvaloniaBinding
    {
        public PropertyClass UntypedProperty { get; }

        public object? GetUntyped(Control control);

        public void SetUntyped(Control control, object? val);

        public bool IsValidControl(Control control);
    }

    private class AvaloniaBinding<TControl, TProperty> : IAvaloniaBinding
    {
        public PropertyClass<TProperty> Property { get; }
        public Func<TControl, TProperty> Getter { get; }
        public Action<TControl, TProperty> Setter { get; }

        public AvaloniaBinding(PropertyClass<TProperty> property,
            Func<TControl, TProperty> getter,
            Action<TControl, TProperty> setter)
        {
            Property = property;
            Getter = getter;
            Setter = setter;
        }

        public TProperty Get(TControl control) => Getter(control);

        public void Set(TControl control, TProperty val) => Setter(control, val);

        public PropertyClass UntypedProperty => Property;

        public object? GetUntyped(Control control)
        {
            if (control is TControl t)
                return Get(t);
            throw new Exception("Invalid type for getting " + Property.Name + ", got " + control.GetType() +
                                " expected " + typeof(TControl));
        }

        public void SetUntyped(Control control, object? val)
        {
            if (control is not TControl t)
                throw new Exception("Invalid type for setting " + Property.Name + ", got " + control.GetType() +
                                    " expected " + typeof(TControl));
            if (val is int && typeof(TProperty).IsEnum)
                val = Enum.ToObject(typeof(TProperty), val);
            if (val is not TProperty v)
                throw new Exception("Invalid value type for setting " + Property.Name + ", got " + val?.GetType() +
                                    " expected " + typeof(TProperty));
            Set(t, v);
        }

        public bool IsValidControl(Control control)
        {
            return control is TControl;
        }
    }

    static AvaloniaInteroperability()
    {
        Register<Control, double>(VBProperties.LeftProperty, w => Canvas.GetLeft(w).OrZero(), Canvas.SetLeft);
        Register<Control, double>(VBProperties.TopProperty, w => Canvas.GetTop(w).OrZero(), Canvas.SetTop);
        Register<Control, double>(VBProperties.WidthProperty, Layoutable.WidthProperty);
        Register<Control, StandardCursorType, Cursor?>(VBProperties.MousePointerProperty, InputElement.CursorProperty, x => new Cursor(x), x => (StandardCursorType)(Enum.ToObject(typeof(StandardCursorType), x?.ToString() ?? "Arrow")));
        Register<Control, bool, FlowDirection>(VBProperties.RightToLeftProperty, Visual.FlowDirectionProperty, x => x ? FlowDirection.RightToLeft : FlowDirection.LeftToRight, x => x == FlowDirection.RightToLeft);
        Register<Control, double>(VBProperties.HeightProperty, Layoutable.HeightProperty);
        Register<Control, bool>(VBProperties.VisibleProperty, Visual.IsVisibleProperty);
        Register<Control, bool>(VBProperties.EnabledProperty, InputElement.IsEnabledProperty);
        Register<TemplatedControl, VBColor, VBColor?>(VBProperties.BackColorProperty, AttachedProperties.BackColorProperty, x => x, x=> x ?? default);
        Register<TemplatedControl, VBColor, VBColor?>(VBProperties.ForeColorProperty, AttachedProperties.ForeColorProperty, x => x, x=> x ?? default);
        Register<TemplatedControl, VBFont, VBFont?>(VBProperties.FontProperty, AttachedProperties.FontProperty, x => x, x=> x ?? default);
        Register<Window, string>(VBProperties.CaptionProperty, (window) => window.Title ?? "", (window, value) => window.Title = value);

        Register<VBCheckBox, VBAppearance>(VBProperties.AppearanceProperty, VBCheckBox.AppearanceProperty);
        Register<VBCheckBox, VBCheckValue>(VBProperties.CheckValueProperty, VBCheckBox.ValueProperty);

        Register<VBLabel, string, string?>(VBProperties.CaptionProperty, VBLabel.TextProperty, x => x, x => x ?? "");
        Register<VBCheckBox, string, object?>(VBProperties.CaptionProperty, ContentControl.ContentProperty, x => x, x => x as string ?? "");
        Register<VBOptionButton, string, object?>(VBProperties.CaptionProperty, ContentControl.ContentProperty, x => x, x => x as string ?? "");
        Register<VBCommandButton, string, object?>(VBProperties.CaptionProperty, ContentControl.ContentProperty, x => x, x => x as string ?? "");

        Register<ComboBox, int>(VBProperties.ListIndexProperty, SelectingItemsControl.SelectedIndexProperty);
        Register<ListBox, int>(VBProperties.ListIndexProperty, SelectingItemsControl.SelectedIndexProperty);
    }

    private static VBColor ToVbColor(IBrush? brush)
    {
        if (brush is not SolidColorBrush solidColorBrush)
            return VBColor.Black;

        return VBColor.FromColor(solidColorBrush.Color);
    }

    private static void Register<TControl, TProperty>(PropertyClass<TProperty> property,
        Func<TControl, TProperty> getter,
        Action<TControl, TProperty> setter)
    {
        var binding = new AvaloniaBinding<TControl, TProperty>(property, getter, setter);
        bindings.Add(binding);
        if (!bindingsByProperty.TryGetValue(property, out var prop))
            prop = bindingsByProperty[property] = new();
        prop.Add(binding);
    }

    private static void Register<TControl, TProperty>(PropertyClass<TProperty> vbProperty, AvaloniaProperty<TProperty> avaloniaProperty) where TControl : Control
    {
        Register<TControl, TProperty>(vbProperty, w =>
        {
            if (w.GetValue(avaloniaProperty) is TProperty prop)
                return prop;
            return default!;
        }, (w, v) => w.SetValue(avaloniaProperty, v));
    }

    private static void Register<TControl, TProperty, TAvaProperty>(PropertyClass<TProperty> vbProperty,
        AvaloniaProperty<TAvaProperty> avaloniaProperty,
        Func<TProperty, TAvaProperty> propertyToAva,
        Func<TAvaProperty, TProperty> avaToProperty) where TControl : Control
    {
        Register<TControl, TProperty>(vbProperty, w =>
        {
            if (w.GetValue(avaloniaProperty) is TAvaProperty prop)
                return avaToProperty(prop);
            return default!;
        }, (w, v) => w.SetValue(avaloniaProperty, propertyToAva(v)));
    }

    public static bool TrySet(Control control, PropertyClass property, Vb6Value value)
    {
        if (bindingsByProperty.TryGetValue(property, out var props))
        {
            foreach (var prop in props)
            {
                if (!prop.IsValidControl(control))
                    continue;

                prop.SetUntyped(control, value.Value);
                return true;
            }
        }

        return false;
    }

    public static bool TryGet(Control c, PropertyClass property, out Vb6Value value)
    {
        if (bindingsByProperty.TryGetValue(property, out var props))
        {
            foreach (var prop in props)
            {
                if (!prop.IsValidControl(c))
                    continue;

                value = FromObject(prop.GetUntyped(c));
                return true;
            }
        }

        value = default!;
        return false;
    }

    private static Vb6Value FromObject(object? untyped)
    {
        if (untyped is null)
            return Vb6Value.Null;
        if (untyped is int i)
            return new Vb6Value(i);
        if (untyped is string s)
            return new Vb6Value(s);
        if (untyped is float f)
            return new Vb6Value(f);
        if (untyped is double d)
            return new Vb6Value(d);
        if (untyped is bool b)
            return new Vb6Value(b);
        if (untyped is VBColor col)
            return new Vb6Value(col);
        if (untyped.GetType().IsEnum)
            return new Vb6Value((int)untyped);
        throw new NotImplementedException("Type " + untyped.GetType() + " is not supported yet");
    }
}