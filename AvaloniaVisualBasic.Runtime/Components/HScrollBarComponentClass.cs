using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using static AvaloniaVisualBasic.Runtime.Components.VBProperties;

namespace AvaloniaVisualBasic.Runtime.Components;

public abstract class ScrollBarComponentClass : ComponentBaseClass
{
    protected ScrollBarComponentClass() : base([CausesValidationProperty,
        SmallChangeProperty,
        LargeChangeProperty,
        MinProperty,
        ValueProperty,
        MaxProperty,
        TabStopProperty,
        TabIndexProperty], [ChangeEvent])
    {
    }

    protected abstract Orientation Orientation { get; }

    public static EventClass ChangeEvent = new EventClass("Change");

    protected override Control InstantiateInternal(ComponentInstance instance)
    {
        return new VBScrollBar()
        {
            Orientation = Orientation,
            Minimum = instance.GetPropertyOrDefault(MinProperty),
            Maximum = instance.GetPropertyOrDefault(MaxProperty),
            Value = instance.GetPropertyOrDefault(ValueProperty),
            LargeChange = instance.GetPropertyOrDefault(LargeChangeProperty),
            SmallChange = instance.GetPropertyOrDefault(SmallChangeProperty),
        };
    }
}

public class HScrollBarComponentClass : ScrollBarComponentClass
{
    public override string Name => "HScroll";
    public override string VBTypeName => "VB.HScrollBar";

    protected override Orientation Orientation => Orientation.Horizontal;

    public static ComponentBaseClass Instance { get; } = new HScrollBarComponentClass();
}