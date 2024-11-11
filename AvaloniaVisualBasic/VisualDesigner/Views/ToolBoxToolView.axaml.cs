using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaVisualBasic.VisualDesigner.Views;

public partial class ToolBoxToolView : UserControl
{
    public ToolBoxToolView()
    {
        InitializeComponent();
    }

    private void OnToolDoubleTap(object? sender, TappedEventArgs e)
    {
        if (e.Source is Control c &&
            c.DataContext is ComponentToolViewModel toolVm &&
            toolVm.BaseClass != null &&
            DataContext is ToolBoxToolViewModel vm)
        {
            vm.SpawnControl(toolVm.BaseClass);
            vm.SelectedComponent = toolVm; //<-- this is confusing, but in order to make sure toolVm gets DEselected, we need to select it first (it will be deselected on PointerRelease)
            e.Handled = true;
        }
    }
}