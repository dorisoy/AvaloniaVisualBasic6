using Avalonia;
using Avalonia.Controls;
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
    ])
    {
    }

    public override string Name => "Picture";
    public override string VBTypeName => "VB.PictureBox";

    protected override Control InstantiateInternal(ComponentInstance instance)
    {
        return new ClassicBorderDecorator()
        {
            BorderStyle = ClassicBorderStyle.Sunken,
            BorderThickness = new Thickness(2)
        };
    }

    public static ComponentBaseClass Instance { get; } = new PictureBoxComponentClass();
}