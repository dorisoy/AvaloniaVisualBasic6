using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic.Controls;

public class PropertyFontBox : TemplatedControl
{
    public static readonly StyledProperty<VBFont> FontProperty = AvaloniaProperty.Register<PropertyFontBox, VBFont>("Font", defaultBindingMode: BindingMode.TwoWay, defaultValue: VBFont.Default);
    public static readonly DirectProperty<PropertyFontBox, string?> FontNameProperty = AvaloniaProperty.RegisterDirect<PropertyFontBox, string?>("FontName", o => o.FontName);

    public string? FontName
    {
        get => Font.FontFamily.Name;
    }

    public VBFont Font
    {
        get => GetValue(FontProperty);
        set => SetValue(FontProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var openFontWindowButton = e.NameScope.Get<Button>("PART_OpenFontWindowButton");
        openFontWindowButton.Click += OnButtonClick;
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        async Task OpenFontWindow()
        {
            var result = await FontDialog.ShowDialog(this.VisualRoot as Window, new FontDialogResult(
                Font.FontFamily, Font.Style, Font.Weight, Font.Size));
            if (result != null)
            {
                SetCurrentValue(FontProperty, new VBFont(result.Family, (int)result.Size, result.Weight, result.Style));
            }
        }

        OpenFontWindow();
    }

    static PropertyFontBox()
    {
        FontProperty.Changed.AddClassHandler<PropertyFontBox>((box, e) =>
        {
            box.RaisePropertyChanged(FontNameProperty, null, box.FontName);
        });
    }
}