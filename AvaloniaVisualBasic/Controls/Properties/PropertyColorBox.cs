using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Controls;

public class PropertyColorBox : TemplatedControl
{
    private ListBox? paletteListBox, systemColorsListBox;
    private bool systemColorListBoxSetting;
    private bool paletteListBoxSetting;
    private bool syncing;

    public static readonly StyledProperty<VBColor> ColorProperty = AvaloniaProperty.Register<PropertyColorBox, VBColor>(nameof(Color), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<VBColor?> SelectedPaletteColorProperty = AvaloniaProperty.Register<PropertyColorBox, VBColor?>(nameof(SelectedPaletteColor), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<VBColor?> SelectedSystemColorProperty = AvaloniaProperty.Register<PropertyColorBox, VBColor?>(nameof(SelectedSystemColor), defaultBindingMode: BindingMode.TwoWay);

    public VBColor Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public VBColor? SelectedPaletteColor
    {
        get => GetValue(SelectedPaletteColorProperty);
        set => SetValue(SelectedPaletteColorProperty, value);
    }

    public VBColor? SelectedSystemColor
    {
        get => GetValue(SelectedSystemColorProperty);
        set => SetValue(SelectedSystemColorProperty, value);
    }

    static PropertyColorBox()
    {
        ColorProperty.Changed.AddClassHandler<PropertyColorBox>((colorBox, e) =>
        {
            if (colorBox.syncing)
                return;
            var color = colorBox.Color;
            colorBox.syncing = true;
            colorBox.SetCurrentValue(SelectedPaletteColorProperty, color);
            colorBox.SetCurrentValue(SelectedSystemColorProperty, color);
            colorBox.syncing = false;
        });
        SelectedPaletteColorProperty.Changed.AddClassHandler<PropertyColorBox>((colorBox, e) =>
        {
            if (colorBox.syncing)
                return;
            colorBox.syncing = true;
            if (colorBox.SelectedPaletteColor is { } clr)
                colorBox.SetCurrentValue(ColorProperty, clr);
            colorBox.syncing = false;
        });
        SelectedSystemColorProperty.Changed.AddClassHandler<PropertyColorBox>((colorBox, e) =>
        {
            if (colorBox.syncing)
                return;
            colorBox.syncing = true;
            if (colorBox.SelectedSystemColor is { } clr)
                colorBox.SetCurrentValue(ColorProperty, clr);
            colorBox.syncing = false;
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        paletteListBox = e.NameScope.Get<ListBox>("PART_PaletteListBox");
        systemColorsListBox = e.NameScope.Get<ListBox>("PART_SystemColorsListBox");
    }

    public IReadOnlyList<VBColor> BuiltinPalette => s_BuiltinPalette;

    public IReadOnlyList<VBColor> SystemColors => s_SystemColors;

    public static List<VBColor> s_SystemColors =
    [
        VBColor.FromSystemColor(VbSystemColor.Scrollbar),
        VBColor.FromSystemColor(VbSystemColor.Desktop),
        VBColor.FromSystemColor(VbSystemColor.Activecaption),
        VBColor.FromSystemColor(VbSystemColor.Inactivecaption),
        VBColor.FromSystemColor(VbSystemColor.Menu),
        VBColor.FromSystemColor(VbSystemColor.Window),
        VBColor.FromSystemColor(VbSystemColor.Windowframe),
        VBColor.FromSystemColor(VbSystemColor.Menutext),
        VBColor.FromSystemColor(VbSystemColor.Windowtext),
        VBColor.FromSystemColor(VbSystemColor.Captiontext),
        VBColor.FromSystemColor(VbSystemColor.Activeborder),
        VBColor.FromSystemColor(VbSystemColor.Inactiveborder),
        VBColor.FromSystemColor(VbSystemColor.Appworkspace),
        VBColor.FromSystemColor(VbSystemColor.Highlight),
        VBColor.FromSystemColor(VbSystemColor.Highlighttext),
        VBColor.FromSystemColor(VbSystemColor.Btnface),
        VBColor.FromSystemColor(VbSystemColor.Btnshadow),
        VBColor.FromSystemColor(VbSystemColor.Graytext),
        VBColor.FromSystemColor(VbSystemColor.Btntext),
        VBColor.FromSystemColor(VbSystemColor.Inactivecaptiontext),
        VBColor.FromSystemColor(VbSystemColor.Btnhighlight),
        VBColor.FromSystemColor(VbSystemColor._3DDKSHADOW),
        VBColor.FromSystemColor(VbSystemColor._3DLIGHT),
        VBColor.FromSystemColor(VbSystemColor.Infotext),
        VBColor.FromSystemColor(VbSystemColor.Infobk)
    ];

    public static List<VBColor> s_BuiltinPalette =
    [
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xC0, 0xC0),
        VBColor.FromColor(0xFF, 0xE0, 0xC2),
        VBColor.FromColor(0xFE, 0xFF, 0xC4),
        VBColor.FromColor(0xB8, 0xFF, 0xC4),
        VBColor.FromColor(0xB9, 0xFF, 0xFF),
        VBColor.FromColor(0xC1, 0xC0, 0xFC),
        VBColor.FromColor(0xFF, 0xC0, 0xFC),
        VBColor.FromColor(0xE0, 0xE0, 0xE0),
        VBColor.FromColor(0xFF, 0x7F, 0x81),
        VBColor.FromColor(0xFF, 0xBF, 0x86),
        VBColor.FromColor(0xFE, 0xFF, 0x8C),
        VBColor.FromColor(0x66, 0xFF, 0x8A),
        VBColor.FromColor(0x6A, 0xFF, 0xFE),
        VBColor.FromColor(0x83, 0x81, 0xFA),
        VBColor.FromColor(0xFF, 0x80, 0xFB),
        VBColor.FromColor(0xC0, 0xC0, 0xC0),
        VBColor.FromColor(0xFF, 0x00, 0x14),
        VBColor.FromColor(0xFF, 0x7F, 0x24),
        VBColor.FromColor(0xFD, 0xFE, 0x43),
        VBColor.FromColor(0x00, 0xFF, 0x3F),
        VBColor.FromColor(0x00, 0xFF, 0xFE),
        VBColor.FromColor(0x20, 0x12, 0xF9),
        VBColor.FromColor(0xFF, 0x06, 0xF9),
        VBColor.FromColor(0x80, 0x80, 0x80),
        VBColor.FromColor(0xC8, 0x00, 0x0C),
        VBColor.FromColor(0xC7, 0x3F, 0x12),
        VBColor.FromColor(0xBF, 0xBF, 0x30),
        VBColor.FromColor(0x00, 0xC0, 0x2D),
        VBColor.FromColor(0x00, 0xC0, 0xBF),
        VBColor.FromColor(0x15, 0x0B, 0xBB),
        VBColor.FromColor(0xC9, 0x03, 0xBC),
        VBColor.FromColor(0x40, 0x40, 0x40),
        VBColor.FromColor(0x85, 0x00, 0x05),
        VBColor.FromColor(0x84, 0x3F, 0x0D),
        VBColor.FromColor(0x7F, 0x80, 0x1D),
        VBColor.FromColor(0x00, 0x80, 0x1B),
        VBColor.FromColor(0x00, 0x80, 0x80),
        VBColor.FromColor(0x0A, 0x04, 0x7D),
        VBColor.FromColor(0x86, 0x01, 0x7D),
        VBColor.FromColor(0x00, 0x00, 0x00),
        VBColor.FromColor(0x43, 0x00, 0x01),
        VBColor.FromColor(0x84, 0x40, 0x41),
        VBColor.FromColor(0x40, 0x40, 0x0A),
        VBColor.FromColor(0x00, 0x40, 0x08),
        VBColor.FromColor(0x00, 0x40, 0x40),
        VBColor.FromColor(0x02, 0x01, 0x3E),
        VBColor.FromColor(0x43, 0x00, 0x3E),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF),
        VBColor.FromColor(0xFF, 0xFF, 0xFF)
    ];
}