using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace AvaloniaVisualBasic.Tools;

public partial class ProjectToolView : UserControl
{
    public ProjectToolView()
    {
        InitializeComponent();
    }

    private void TreeView_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is Control c &&
            c.FindAncestorOfType<TreeViewItem>(true) != null)
        {
            if (DataContext is ProjectToolViewModel vm)
            {
                vm.OpenSelected();
            }
        }
    }
}