using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AvaloniaVisualBasic.Controls;

public class ResizeAdorner : TemplatedControl
{
    public static readonly AttachedProperty<ResizeXDirection> ResizeXDirectionProperty = AvaloniaProperty.RegisterAttached<ResizeAdorner, Control, ResizeXDirection>("ResizeXDirection");
    public static ResizeXDirection GetResizeXDirection(AvaloniaObject element) => element.GetValue(ResizeXDirectionProperty);
    public static void SetResizeXDirection(AvaloniaObject element, ResizeXDirection value) => element.SetValue(ResizeXDirectionProperty, value);

    public static readonly AttachedProperty<ResizeYDirection> ResizeYDirectionProperty = AvaloniaProperty.RegisterAttached<ResizeAdorner, Control, ResizeYDirection>("ResizeYDirection");
    public static ResizeYDirection GetResizeYDirection(AvaloniaObject element) => element.GetValue(ResizeYDirectionProperty);
    public static void SetResizeYDirection(AvaloniaObject element, ResizeYDirection value) => element.SetValue(ResizeYDirectionProperty, value);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var topLeft = e.NameScope.Get<Control>("PART_TopLeft");
        var top = e.NameScope.Get<Control>("PART_Top");
        var topRight = e.NameScope.Get<Control>("PART_TopRight");
        var left = e.NameScope.Get<Control>("PART_Left");
        var right = e.NameScope.Get<Control>("PART_Right");
        var bottom = e.NameScope.Get<Control>("PART_Bottom");
        var bottomLeft = e.NameScope.Get<Control>("PART_BottomLeft");
        var bottomRight = e.NameScope.Get<Control>("PART_BottomRight");
        var moveGrip = e.NameScope.Find<Control>("PART_MoveGrip");

        var all = new Control[]{topLeft, top, topRight, left, right, bottom, bottomLeft, bottomRight};

        foreach (var control in all)
        {
            control.AddHandler(PointerPressedEvent, OnHandlePointerPressed);
            control.AddHandler(PointerMovedEvent, OnHandlePointerMoved);
            control.AddHandler(PointerReleasedEvent, OnHandlePointerReleased);
        }

        if (moveGrip != null)
        {
            moveGrip.AddHandler(PointerPressedEvent, OnHandlePointerPressed);
            moveGrip.AddHandler(PointerMovedEvent, OnHandlePointerMoved);
            moveGrip.AddHandler(PointerReleasedEvent, OnHandlePointerReleased);
        }
    }

    private bool isResizing = false;
    private Point initialPosition;
    private Rect originalBounds;
    private Point originalOrigin;
    private ResizeXDirection xDirection = ResizeXDirection.None;
    private ResizeYDirection yDirection = ResizeYDirection.None;
    private Visual? adornedElement;
    private Control? adornedElementChild;

    private void OnHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        isResizing = false;
        adornedElement = null;
        adornedElementChild = null;
    }

    private void OnHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (isResizing && adornedElement != null)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                isResizing = false;
                adornedElement = null;
                return;
            }
            var position = e.GetPosition(this.GetVisualParent());
            var diff = position - initialPosition;

            double newWidth = originalBounds.Width, newHeight = originalBounds.Height, newTop = originalOrigin.Y, newLeft = originalOrigin.X;

            double SnapToGrid(double x)
            {
                return SnapGridUtils.SnapToGrid(this, x);
            }

            if (xDirection == ResizeXDirection.Right)
            {
                newWidth = SnapToGrid(originalBounds.Width + diff.X);
            }
            else if (xDirection == ResizeXDirection.Left)
            {
                var originalRight = originalOrigin.X + originalBounds.Width;
                newLeft = Math.Clamp(SnapToGrid(originalOrigin.X + diff.X), 0, originalRight);
                newWidth = originalRight - newLeft;
            }


            if (yDirection == ResizeYDirection.Bottom)
            {
                newHeight = SnapToGrid(originalBounds.Height + diff.Y);
            }
            else if (yDirection == ResizeYDirection.Top)
            {
                var originalBottom = originalOrigin.Y + originalBounds.Height;
                newTop = Math.Clamp(SnapToGrid(originalOrigin.Y + diff.Y), 0, originalBottom);
                newHeight = originalBottom - newTop;
            }

            if (xDirection == ResizeXDirection.None && yDirection == ResizeYDirection.None)
            {
                newLeft = SnapToGrid(originalOrigin.X + diff.X);
                newTop = SnapToGrid(originalOrigin.Y + diff.Y);
            }

            if (adornedElementChild != null)
            {
                if (adornedElementChild.MinHeight != 0)
                    newHeight = Math.Max(adornedElementChild.MinHeight, newHeight);
                if (adornedElementChild.MaxHeight != 0)
                    newHeight = Math.Min(adornedElementChild.MaxHeight, newHeight);
                if (adornedElementChild.MinWidth != 0)
                    newWidth = Math.Max(adornedElementChild.MinWidth, newWidth);
                if (adornedElementChild.MaxWidth != 0)
                    newWidth = Math.Min(adornedElementChild.MaxWidth, newWidth);
            }

            adornedElement.SetCurrentValue(Canvas.TopProperty, newTop);
            adornedElement.SetCurrentValue(HeightProperty, Math.Max(1, newHeight));
            adornedElement.SetCurrentValue(Canvas.LeftProperty, newLeft);
            adornedElement.SetCurrentValue(WidthProperty, Math.Max(1, newWidth));
        }
    }

    private void OnHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            adornedElement = AdornerLayer.GetAdornedElement(this);
            adornedElementChild = null;
            if (adornedElement != null && e.Source is Control control)
            {
                if (adornedElement is ControlItem controlItem)
                    adornedElementChild = controlItem.Presenter?.Child;

                isResizing = true;
                initialPosition = e.GetPosition(this.GetVisualParent());
                originalBounds = adornedElement.Bounds;
                originalOrigin = new Point(double.IsNaN(Canvas.GetLeft(adornedElement)) ? 0 : Canvas.GetLeft(adornedElement),
                    double.IsNaN(Canvas.GetTop(adornedElement)) ? 0 : Canvas.GetTop(adornedElement));
                xDirection = GetResizeXDirection(control);
                yDirection = GetResizeYDirection(control);
                e.Pointer.Capture(this);
            }
        }
    }

    public void StartDrag(PointerPressedEventArgs e)
    {
        OnHandlePointerPressed(null, e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        OnHandlePointerMoved(null, e);
    }
}

public enum ResizeXDirection
{
    None,
    Right,
    Left
}

public enum ResizeYDirection
{
    None,
    Bottom,
    Top
}