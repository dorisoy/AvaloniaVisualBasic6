using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AvaloniaVisualBasic.Controls;

public class ControlItem : ListBoxItem
{
    static ControlItem()
    {
        IsSelectedProperty.Changed.AddClassHandler<ControlItem>((item, e) =>
        {
            if (item.IsSelected)
            {
                var adorner = new ResizeAdorner();
                AdornerLayer.SetAdorner(item, adorner);
            }
            else
            {
                AdornerLayer.SetAdorner(item, null);
            }
        });
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (IsSelected && AdornerLayer.GetAdorner(this) is ResizeAdorner adorner)
        {
            adorner.StartDrag(e);
        }
    }
}