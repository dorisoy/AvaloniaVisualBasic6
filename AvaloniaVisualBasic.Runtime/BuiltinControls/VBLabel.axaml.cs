using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;
using Classic.Avalonia.Theme;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBLabel : TemplatedControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<VBLabel, string?>(nameof(Text));

    public static readonly StyledProperty<VBColor> BackColorProperty = AvaloniaProperty.Register<VBLabel, VBColor>(nameof(BackColor));

    public static readonly StyledProperty<BackStyles> BackStyleProperty = AvaloniaProperty.Register<VBLabel, BackStyles>(nameof(BackStyle));

    public static readonly StyledProperty<VBAppearance> AppearanceProperty = AvaloniaProperty.Register<VBLabel, VBAppearance>(nameof(Appearance));

    public static readonly StyledProperty<VBBorder> BorderStyleProperty = AvaloniaProperty.Register<VBLabel, VBBorder>(nameof(BorderStyle));

    public static readonly StyledProperty<VBTextAlignment> AlignmentProperty = AvaloniaProperty.Register<VBLabel, VBTextAlignment>(nameof(Alignment));

    public static readonly StyledProperty<bool> WordWrapProperty = AvaloniaProperty.Register<VBLabel, bool>(nameof(WordWrap));

    public static readonly StyledProperty<bool> RecognizesAccessKeyProperty = AvaloniaProperty.Register<VBLabel, bool>(nameof(RecognizesAccessKey));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public VBColor BackColor
    {
        get => GetValue(BackColorProperty);
        set => SetValue(BackColorProperty, value);
    }

    public BackStyles BackStyle
    {
        get => GetValue(BackStyleProperty);
        set => SetValue(BackStyleProperty, value);
    }

    public VBAppearance Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    public VBBorder BorderStyle
    {
        get => GetValue(BorderStyleProperty);
        set => SetValue(BorderStyleProperty, value);
    }

    public VBTextAlignment Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    public bool WordWrap
    {
        get => GetValue(WordWrapProperty);
        set => SetValue(WordWrapProperty, value);
    }

    public bool RecognizesAccessKey
    {
        get => GetValue(RecognizesAccessKeyProperty);
        set => SetValue(RecognizesAccessKeyProperty, value);
    }

    private ClassicBorderDecorator? decorator;
    private TextBlock? textblock;
    private AccessText? accessText;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        decorator = e.NameScope.Get<ClassicBorderDecorator>("PART_Border");
        UpdateStyle();
    }

    static VBLabel()
    {
        AffectsRender<VBLabel>(BackColorProperty, BackStyleProperty);
        AppearanceProperty.Changed.AddClassHandler<VBLabel>((label, e) => label.UpdateStyle());
        BorderStyleProperty.Changed.AddClassHandler<VBLabel>((label, e) => label.UpdateStyle());
        AlignmentProperty.Changed.AddClassHandler<VBLabel>((label, e) => label.UpdateStyle());
        WordWrapProperty.Changed.AddClassHandler<VBLabel>((label, e) => label.UpdateStyle());
        RecognizesAccessKeyProperty.Changed.AddClassHandler<VBLabel>((label, e) => label.UpdateStyle());
        TextProperty.Changed.AddClassHandler<VBLabel>((label, e) => label.UpdateStyle());

        AttachedEvents.AttachClick<VBLabel>();
    }

    private void UpdateStyle()
    {
        if (decorator == null)
            return;

        if (BorderStyle == VBBorder.None)
        {
            decorator.BorderThickness = new Thickness(0);
        }
        else if (BorderStyle == VBBorder.FixedSingle)
        {
            if (Appearance == VBAppearance.Flat)
            {
                decorator.BorderStyle = ClassicBorderStyle.Sunken;
                decorator.BorderThickness = new Thickness(1);
                decorator.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            else if (Appearance == VBAppearance._3D)
            {
                decorator.BorderStyle = ClassicBorderStyle.Sunken;
                decorator.BorderThickness = new Thickness(2);
                decorator.BorderBrush = ClassicBorderDecorator.ClassicBorderBrush;
            }
        }

        if (RecognizesAccessKey && (decorator.Child == null || decorator.Child is TextBlock))
        {
            decorator.Child = new AccessText();
        }
        else if (!RecognizesAccessKey && (decorator.Child == null || decorator.Child is AccessText))
        {
            decorator.Child = new TextBlock();
        }

        var textblock = (decorator.Child as TextBlock)!;
        textblock.Text = RecognizesAccessKey && Text != null && Text.Contains('&') ? Text.Replace("&", "_") : Text;
        textblock.TextWrapping = WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
        textblock.TextAlignment = Alignment switch
        {
            VBTextAlignment.LeftJustify => TextAlignment.Left,
            VBTextAlignment.RightJustify => TextAlignment.Right,
            VBTextAlignment.Center => TextAlignment.Center,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void Render(DrawingContext context)
    {
        if (BackStyle == BackStyles.Transparent)
            return;

        if (BackStyle == BackStyles.Opaque)
            context.FillRectangle(BackColor.ToBrush(), new Rect(0, 0, Bounds.Width, Bounds.Height));
    }
}