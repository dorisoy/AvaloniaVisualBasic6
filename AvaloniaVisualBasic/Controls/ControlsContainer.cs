using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaVisualBasic.Controls;

public class ControlsContainer : ListBox
{
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ControlItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<ControlItem>(item, out recycleKey);
    }
}