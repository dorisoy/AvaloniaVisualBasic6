using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using Classic.Avalonia.Theme;
using static AvaloniaVisualBasic.Runtime.Components.VBProperties;

namespace AvaloniaVisualBasic.Runtime.Components;

public class PictureBoxComponentClass : ComponentBaseClass
{
    public PictureBoxComponentClass() : base([AlignProperty,
    AppearanceProperty, AutoRedrawProperty, AutoSizeProperty,
    BackColorProperty, BorderStyleProperty,
    CausesValidationProperty, EnabledProperty,
    FillColorProperty, FillStyleProperty,
    FontProperty, ForeColorProperty,
    MousePointerProperty, ToolTipTextProperty, PictureProperty,
    ], [ClickEvent])
    {
    }

    public override string Name => "Picture";
    public override string VBTypeName => "VB.PictureBox";

    protected override Control InstantiateInternal(ComponentInstance instance)
    {
        return new VBPictureBox()
        {
            [AttachedProperties.BackColorProperty] = instance.GetPropertyOrDefault(BackColorProperty),
            [AttachedProperties.ForeColorProperty] = instance.GetPropertyOrDefault(ForeColorProperty),
            [AttachedProperties.FontProperty] = instance.GetPropertyOrDefault(FontProperty),
            Cursor = new Cursor(instance.GetPropertyOrDefault(MousePointerProperty)),
            FlowDirection = instance.GetPropertyOrDefault(RightToLeftProperty) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
        };
    }

    public static ComponentBaseClass Instance { get; } = new PictureBoxComponentClass();
}