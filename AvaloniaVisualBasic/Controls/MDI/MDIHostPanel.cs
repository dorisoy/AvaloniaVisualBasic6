using System;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaVisualBasic.Utils;

namespace AvaloniaVisualBasic.Controls;

public class MDIHostPanel : Panel
{
    public static readonly AttachedProperty<bool> IsActiveProperty = AvaloniaProperty.RegisterAttached<MDIHostPanel, Control, bool>("IsActive");
    public static readonly AttachedProperty<WindowState> WindowStateProperty = AvaloniaProperty.RegisterAttached<MDIHostPanel, Control, WindowState>("WindowState");
    public static readonly AttachedProperty<Point> WindowLocationProperty = AvaloniaProperty.RegisterAttached<MDIHostPanel, Control, Point>("WindowLocation");
    public static readonly AttachedProperty<Point> OldWindowLocationProperty = AvaloniaProperty.RegisterAttached<MDIHostPanel, Control, Point>("OldWindowLocation");
    public static readonly AttachedProperty<Point?> OldMinimizedWindowLocationProperty = AvaloniaProperty.RegisterAttached<MDIHostPanel, Control, Point?>("OldMinimizedWindowLocation");
    public static readonly AttachedProperty<Size> WindowSizeProperty = AvaloniaProperty.RegisterAttached<MDIHostPanel, Control, Size>("WindowSize");

    public static bool GetIsActive(AvaloniaObject element) => element.GetValue(IsActiveProperty);

    public static void SetIsActive(AvaloniaObject element, bool value) => element.SetValue(IsActiveProperty, value);

    public static Point GetWindowLocation(AvaloniaObject element) => element.GetValue(WindowLocationProperty);

    public static void SetWindowLocation(AvaloniaObject element, Point value) => element.SetValue(WindowLocationProperty, value);

    public static Point? GetOldMinimizedWindowLocation(AvaloniaObject element) => element.GetValue(OldMinimizedWindowLocationProperty);

    public static void SetOldMinimizedWindowLocation(AvaloniaObject element, Point? value) => element.SetValue(OldMinimizedWindowLocationProperty, value);

    public static Point GetOldWindowLocation(AvaloniaObject element) => element.GetValue(OldWindowLocationProperty);

    public static void SetOldWindowLocation(AvaloniaObject element, Point value) => element.SetValue(OldWindowLocationProperty, value);

    public static WindowState GetWindowState(AvaloniaObject element) => element.GetValue(WindowStateProperty);

    public static void SetWindowState(AvaloniaObject element, WindowState value) => element.SetValue(WindowStateProperty, value);

    public static Size GetWindowSize(AvaloniaObject element) => element.GetValue(WindowSizeProperty);

    public static void SetWindowSize(AvaloniaObject element, Size value) => element.SetValue(WindowSizeProperty, value);

    static MDIHostPanel()
    {
        AffectsParentMeasure<MDIHostPanel>(WindowLocationProperty, WindowSizeProperty, WindowStateProperty, ZIndexProperty);
        AffectsParentArrange<MDIHostPanel>(WindowLocationProperty, WindowSizeProperty, WindowStateProperty, ZIndexProperty);
    }

    private IDisposable? hostWindowIsActiveDisposable;

    public MDIHostPanel()
    {
        Children.CollectionChanged += OnChildrenChanged;
        AddHandler(MDIHost.ActivateWindowEvent, ActivateWindowEventHandler);
    }

    private void ActivateWindowEventHandler(object? sender, RoutedEventArgs e)
    {
        var maxZOrder = 0;
        foreach (var child in Children)
        {
            child.ZIndex--;
            maxZOrder = Math.Max(maxZOrder, child.ZIndex);
        }

        if (e.Source is Control c && c.FindAncestorOfType<MDIWindow>(true) is {} window)
        {
            window.ZIndex = maxZOrder + 1;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            hostWindowIsActiveDisposable = window.GetObservable(Window.IsActiveProperty)
                .Subscribe(new ActionObserver<bool>(@is =>
                {
                    InvalidateArrange();
                }));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        hostWindowIsActiveDisposable?.Dispose();
        hostWindowIsActiveDisposable = null;
    }

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var topZ = Children.Select(x => x.ZIndex).DefaultIfEmpty().Max();
            var i = Children.Count - e.NewItems!.Count;
            foreach (Control @new in e.NewItems!)
            {
                if (@new is MDIBlockerControl)
                {
                    @new.ZIndex = topZ + 1;
                    continue;
                }

                var location = GetWindowLocation(@new);
                var size = GetWindowSize(@new);
                if (location == default)
                {
                    i++;
                    var point = new Point(30, 30) * i;
                    SetWindowLocation(@new, point);
                    @new.ZIndex = topZ + 1;
                    @new.Focus();
                }

                if (size == default)
                {
                    @new.Measure(Bounds.Size == default ? Size.Infinity : Bounds.Size); // before the MDI host is attached, it can be 0,0 size, not sure what is the best way to solve it
                    SetWindowSize(@new, @new.DesiredSize);
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (Control old in e.OldItems!)
            {
                if (old is MDIWindow mdiWindow && GetIsActive(old))
                {
                    // Raise on `This`, not the `mdiWindow`, because the mdiWindow is already detached from the visual tree
                    RaiseEvent(new ActiveWidowChangedEventArgs(MDIHost.ActiveWindowChangedEvent, old, mdiWindow, false));
                }
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            var state = GetWindowState(child);
            var size = GetWindowSize(child);
            if (state == WindowState.Minimized)
                size = new Size(160, 24);
            else if (state == WindowState.Maximized)
                size = availableSize;
            else
                size = new Size(Math.Max(size.Width, child.MinWidth), Math.Max(size.Height, child.MinHeight));
            child.Measure(size);
        }
        return new Size(0, 0);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Control? topZ = null;
        Control? oldActive = Children.FirstOrDefault(GetIsActive);
        foreach (var child in Children)
        {
            if (child is MDIBlockerControl)
                continue;

            if (topZ == null || topZ.ZIndex < child.ZIndex)
                topZ = child;
        }

        if (oldActive != topZ)
        {
            foreach (var child in Children)
            {
                if (child is MDIBlockerControl)
                    continue;

                SetIsActive(child, false);
                if (child is MDIWindow mdiWindow)
                    child.RaiseEvent(new ActiveWidowChangedEventArgs(MDIHost.ActiveWindowChangedEvent, child, mdiWindow, false));
            }

            if (topZ != null && (this.GetVisualRoot() is Window { IsActive: true } || this.GetVisualRoot() is not Window))
            {
                SetIsActive(topZ, true);
                if (topZ is MDIWindow mdiWindow)
                    topZ.RaiseEvent(new ActiveWidowChangedEventArgs(MDIHost.ActiveWindowChangedEvent, topZ, mdiWindow, true));
            }
        }

        foreach (var child in Children.ToList())
        {
            if (child is MDIBlockerControl blocker)
            {
                blocker.Arrange(new Rect(default, finalSize));
                continue;
            }

            var state = GetWindowState(child);
            var location = GetWindowLocation(child);
            var size = GetWindowSize(child);
            if (state == WindowState.Minimized)
            {
                size = new Size(160, 24);
            }
            else if (state == WindowState.Maximized)
            {
                location = new Point(0, 0);
                size = finalSize;
            }
            else
            {
                size = new Size(Math.Max(size.Width, child.MinWidth), Math.Max(size.Height, child.MinHeight));
            }
            child.Arrange(new Rect(location, size));
        }

        return finalSize;
    }
}