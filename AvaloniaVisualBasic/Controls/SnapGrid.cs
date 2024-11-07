using Avalonia;
using Avalonia.Controls;

namespace AvaloniaVisualBasic.Controls;

public class SnapGridUtils
{
    public static readonly AttachedProperty<bool> SnapToGridProperty = AvaloniaProperty.RegisterAttached<ResizeAdorner, Control, bool>("SnapToGrid", inherits: true);
    public static bool GetSnapToGrid(AvaloniaObject element) => element.GetValue(SnapToGridProperty);
    public static void SetSnapToGrid(AvaloniaObject element, bool value) => element.SetValue(SnapToGridProperty, value);

    public static readonly AttachedProperty<int> GridSizeProperty = AvaloniaProperty.RegisterAttached<ResizeAdorner, Control, int>("GridSize", inherits: true);
    public static int GetGridSize(AvaloniaObject element) => element.GetValue(GridSizeProperty);
    public static void SetGridSize(AvaloniaObject element, int value) => element.SetValue(GridSizeProperty, value);

    public static double SnapToGrid(Control control, double x)
    {
        if (!SnapGridUtils.GetSnapToGrid(control))
            return x;
        var gridSize = SnapGridUtils.GetGridSize(control);
        if (gridSize == 0)
            return x;
        int y = (int)x;
        return y / gridSize * gridSize;
    }
}