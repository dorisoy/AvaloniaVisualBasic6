using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using static AvaloniaVisualBasic.Runtime.Components.VBProperties;

namespace AvaloniaVisualBasic.Runtime.Components;

public class CommandButtonComponentClass : ComponentBaseClass
{
    public CommandButtonComponentClass() : base([CaptionProperty,
        BackColorProperty,
        ForeColorProperty,
        AppearanceProperty,
        FontProperty,
        MousePointerProperty,
        EnabledProperty,
        TabStopProperty,
        TabIndexProperty])
    {
    }

    public override string Name => "Command";
    public override string VBTypeName => "VB.CommandButton";

    protected override Control InstantiateInternal(ComponentInstance instance)
    {
        return new VBCommandButton()
        {
            Content = instance.GetPropertyOrDefault(CaptionProperty),
            [AttachedProperties.BackColorProperty] = instance.GetPropertyOrDefault(BackColorProperty),
            [AttachedProperties.ForeColorProperty] = instance.GetPropertyOrDefault(ForeColorProperty),
            [AttachedProperties.FontProperty] = instance.GetPropertyOrDefault(FontProperty),
            Cursor = new Cursor(instance.GetPropertyOrDefault(MousePointerProperty)),
        };
    }

    public static CommandButtonComponentClass Instance { get; } = new();
}