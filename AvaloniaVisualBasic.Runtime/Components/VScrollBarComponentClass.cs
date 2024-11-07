using Avalonia.Layout;

namespace AvaloniaVisualBasic.Runtime.Components;

public class VScrollBarComponentClass : ScrollBarComponentClass
{
    public override string Name => "VScroll";
    public override string VBTypeName => "VB.VScrollBar";

    protected override Orientation Orientation => Orientation.Vertical;

    public static ComponentBaseClass Instance { get; } = new VScrollBarComponentClass();
}