using Avalonia.Controls;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using static AvaloniaVisualBasic.Runtime.Components.VBProperties;

namespace AvaloniaVisualBasic.Runtime.Components;

public class ShapeComponentClass : ComponentBaseClass
{
    public ShapeComponentClass() : base([BackColorProperty, BackStyleProperty, ShapeProperty, BorderWidthProperty, FillColorProperty, BorderColorProperty, ShapeBorderStyleProperty, FillStyleProperty])
    {
    }

    public override string Name => "Shape";
    public override string VBTypeName => "VB.Shape";

    protected override Control InstantiateInternal(ComponentInstance instance)
    {
        return new VBShape()
        {
            ShapeType = instance.GetPropertyOrDefault(ShapeProperty),
            BorderWidth = instance.GetPropertyOrDefault(BorderWidthProperty),
            FillColor = instance.GetPropertyOrDefault(FillColorProperty),
            BorderColor = instance.GetPropertyOrDefault(BorderColorProperty),
            BorderStyle = instance.GetPropertyOrDefault(ShapeBorderStyleProperty),
            FillStyle = instance.GetPropertyOrDefault(FillStyleProperty),
            BackColor = instance.GetPropertyOrDefault(BackColorProperty),
            BackStyle = instance.GetPropertyOrDefault(BackStyleProperty),
        };
    }

    public static ComponentBaseClass Instance { get; } = new ShapeComponentClass();
}