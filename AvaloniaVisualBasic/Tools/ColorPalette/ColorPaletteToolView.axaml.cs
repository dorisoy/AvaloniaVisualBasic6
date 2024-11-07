using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Tools;

public partial class ColorPaletteToolView : UserControl
{
    public ColorPaletteToolView()
    {
        InitializeComponent();
    }

    private void PickForeColor(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is ColorPaletteToolViewModel vm)
        {
            vm.PickingBackColor = false;
            e.Handled = true;
        }
    }

    private void PickBackColor(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is ColorPaletteToolViewModel vm)
        {
            vm.PickingBackColor = true;
            e.Handled = true;
        }
    }

    private void ColorView_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (DataContext is ColorPaletteToolViewModel vm)
        {
            if (vm.SelectedColor == null)
                return;
            vm.SelectedColor!.Color = VBColor.FromColor(ColorView.Color);
        }
    }
}