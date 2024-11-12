using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using static AvaloniaVisualBasic.Runtime.Components.VBProperties;

namespace AvaloniaVisualBasic.Runtime.Components;

public abstract class ComponentBaseClass
{
    public ComponentBaseClass(IReadOnlyList<PropertyClass> extendedProperties,
        IReadOnlyList<EventClass>? events = null)
    {
        Properties = new List<PropertyClass>(extendedProperties)
        {
            NameProperty,
            LeftProperty,
            TopProperty,
            WidthProperty,
            HeightProperty,
            VisibleProperty,
            TagProperty
        };
        PropertiesByName = Properties.ToDictionary(p => p.Name, p => p);
        Events = events ?? [];
    }

    public IReadOnlyList<PropertyClass> Properties { get; }

    public IReadOnlyDictionary<string, PropertyClass> PropertiesByName { get; }

    public IReadOnlyList<EventClass> Events { get; }

    public abstract string Name { get; }
    public abstract string VBTypeName { get; }

    protected abstract Control InstantiateInternal(ComponentInstance instance);

    public Control Instantiate(ComponentInstance instance)
    {
        var control = InstantiateInternal(instance);
        control.IsEnabled = instance.GetPropertyOrDefault(EnabledProperty);
        ToolTip.SetTip(control, instance.GetPropertyOrDefault(ToolTipTextProperty));
        if (instance.GetPropertyOrDefault(TagProperty) is {} tag)
            control.Tag = tag;
        KeyboardNavigation.SetTabIndex(control, instance.GetPropertyOrDefault(TabIndexProperty));
        KeyboardNavigation.SetIsTabStop(control, instance.GetPropertyOrDefault(TabStopProperty));
        VBProps.SetName(control, instance.GetPropertyOrDefault(NameProperty));
        return control;
    }

    public static EventClass ClickEvent = new EventClass("Click");
    public static EventClass GotFocusEvent = new EventClass("GotFocus");
    public static EventClass LostFocusEvent = new EventClass("LostFocus");
    public static EventClass KeyDownEvent = new EventClass("KeyDown", new EventClassArgument("KeyCode", "Integer"), new EventClassArgument("Shift", "Integer"));
    public static EventClass KeyPressEvent = new EventClass("KeyPress", new EventClassArgument("KeyAscii", "Integer"));
    public static EventClass KeyUpEvent = new EventClass("KeyUp", new EventClassArgument("KeyCode", "Integer"), new EventClassArgument("Shift", "Integer"));
    public static EventClass MouseDownEvent = new EventClass("MouseDown", new EventClassArgument("Button", "Integer"), new EventClassArgument("Shift", "Integer"), new EventClassArgument("X", "Single"), new EventClassArgument("Y", "Single"));
    public static EventClass MouseMoveEvent = new EventClass("MouseMove", new EventClassArgument("Button", "Integer"), new EventClassArgument("Shift", "Integer"), new EventClassArgument("X", "Single"), new EventClassArgument("Y", "Single"));
    public static EventClass MouseUpEvent = new EventClass("MouseUp", new EventClassArgument("Button", "Integer"), new EventClassArgument("Shift", "Integer"), new EventClassArgument("X", "Single"), new EventClassArgument("Y", "Single"));
}