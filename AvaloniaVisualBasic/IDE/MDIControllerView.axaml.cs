using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaVisualBasic.Controls;
using AvaloniaVisualBasic.Events;

namespace AvaloniaVisualBasic.IDE;

public partial class MDIControllerView : UserControl
{
    private IDisposable? sub;

    public MDIControllerView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        sub?.Dispose();
        if (DataContext is MDIControllerViewModel vm)
        {
            sub = vm.EventBus.Subscribe<RearrangeMDIEvent>(e =>
            {
                if (e.Kind == MDIRearrangeKind.TileHorizontally)
                    MDIHost.Tile(Orientation.Horizontal);
                else if (e.Kind == MDIRearrangeKind.TileVertically)
                    MDIHost.Tile(Orientation.Vertical);
                else if (e.Kind == MDIRearrangeKind.Cascade)
                    MDIHost.Cascade();
            });
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        sub?.Dispose();
    }

    private void ActiveWindowChanged(object? sender, ActiveWidowChangedEventArgs e)
    {
        if (DataContext is MDIControllerViewModel vm)
        {
            var relatedDocument = e.Window.DataContext as IMdiWindow;
            if (relatedDocument == null)
                return;

            if (vm.MdiWindowManager.ActiveWindow == relatedDocument)
            {
                if (!e.IsActive)
                    vm.MdiWindowManager.ActiveWindow = null;
            }
            else
            {
                if (e.IsActive)
                {
                    vm.MdiWindowManager.ActiveWindow = relatedDocument;
                }
            }
        }
    }
}