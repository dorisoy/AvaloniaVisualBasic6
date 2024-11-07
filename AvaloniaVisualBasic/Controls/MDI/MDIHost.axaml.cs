using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AvaloniaVisualBasic.Controls;

public class MDIHost : ItemsControl
{
    public static readonly RoutedEvent<ActiveWidowChangedEventArgs> ActiveWindowChangedEvent =
        RoutedEvent.Register<MDIHost, ActiveWidowChangedEventArgs>(nameof(ActiveWindowChanged),
            RoutingStrategies.Bubble | RoutingStrategies.Tunnel);

    public event EventHandler<ActiveWidowChangedEventArgs> ActiveWindowChanged
    {
        add => AddHandler(ActiveWindowChangedEvent, value);
        remove => RemoveHandler(ActiveWindowChangedEvent, value);
    }

    public static readonly RoutedEvent<RoutedEventArgs> ActivateWindowEvent =
        RoutedEvent.Register<MDIHost, RoutedEventArgs>(nameof(ActivateWindow),
            RoutingStrategies.Bubble | RoutingStrategies.Tunnel);

    public event EventHandler<RoutedEventArgs> ActivateWindow
    {
        add => AddHandler(ActivateWindowEvent, value);
        remove => RemoveHandler(ActivateWindowEvent, value);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<MDIWindow>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new MDIWindow();
    }

    public void Tile(Orientation orientation)
    {
        var windowCount = ItemCount;
        if (windowCount == 0)
            return;

        var bounds = Bounds;
        var windowSize = orientation == Orientation.Horizontal
            ? new Size(bounds.Width / windowCount, bounds.Height)
            : new Size(bounds.Width, bounds.Height / windowCount);

        var x = 0.0;
        var y = 0.0;
        for (int i = 0; i < windowCount; ++i)
        {
            var container = ContainerFromIndex(i);
            MDIHostPanel.SetWindowState(container, WindowState.Normal);
            MDIHostPanel.SetWindowLocation(container, orientation == Orientation.Horizontal ? new Point(x, 0) : new Point(0, y));
            MDIHostPanel.SetWindowSize(container, windowSize);
            x += windowSize.Width;
            y += windowSize.Height;
        }
    }

    public void Cascade()
    {
        var windowCount = ItemCount;
        if (windowCount == 0)
            return;

        var windowSize = Bounds.Size / 2;

        var x = 0.0;
        var y = 0.0;
        for (int i = 0; i < windowCount; ++i)
        {
            x += 20;
            y += 20;
            var container = ContainerFromIndex(i);
            MDIHostPanel.SetWindowState(container, WindowState.Normal);
            MDIHostPanel.SetWindowLocation(container, new Point(x, y));
            MDIHostPanel.SetWindowSize(container, windowSize);
        }
    }
}

public class ActiveWidowChangedEventArgs : RoutedEventArgs
{
    public ActiveWidowChangedEventArgs(RoutedEvent? routedEvent, object? source, MDIWindow window, bool isActive) : base(routedEvent, source)
    {
        Window = window;
        IsActive = isActive;
    }

    public MDIWindow Window { get; }
    public bool IsActive { get; }
}