using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaVisualBasic.Controls;
using AvaloniaVisualBasic.Events;

namespace AvaloniaVisualBasic.VisualDesigner;

public partial class FormEditView : UserControl
{
    public FormEditView()
    {
        InitializeComponent();
        NewControlSpawner.AddHandler(PointerPressedEvent, OnNewControlStarted);
        NewControlSpawner.AddHandler(PointerMovedEvent, OnNewControlMoving);
        NewControlSpawner.AddHandler(PointerReleasedEvent, OnNewPointerReleased);
    }

    private double SnapToGrid(double x)
    {
        return SnapGridUtils.SnapToGrid(this, x);
    }

    private Point newControlInitialPress;
    private bool isSpawningNewControl;
    private IDisposable? activateSub;

    private void OnNewPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (isSpawningNewControl && NewControlSpawnerPhantom.IsVisible)
        {
            if (DataContext is FormEditViewModel vm)
                vm.SpawnControl(new Rect(Canvas.GetLeft(NewControlSpawnerPhantom),
                    Canvas.GetTop(NewControlSpawnerPhantom), NewControlSpawnerPhantom.Width,
                    NewControlSpawnerPhantom.Height));
            NewControlSpawnerPhantom.IsVisible = false;
        }
        isSpawningNewControl = false;
    }

    private void OnNewControlMoving(object? sender, PointerEventArgs e)
    {
        if (isSpawningNewControl)
        {
            var point = e.GetCurrentPoint(NewControlSpawner).Position;
            double left = SnapToGrid(Math.Min(point.X, newControlInitialPress.X));
            double top = SnapToGrid(Math.Min(point.Y, newControlInitialPress.Y));
            double right = SnapToGrid(Math.Max(point.X, newControlInitialPress.X));
            double bottom = SnapToGrid(Math.Max(point.Y, newControlInitialPress.Y));
            Canvas.SetLeft(NewControlSpawnerPhantom, left);
            Canvas.SetTop(NewControlSpawnerPhantom, top);
            NewControlSpawnerPhantom.Width = Math.Max(1, right - left);
            NewControlSpawnerPhantom.Height = Math.Max(1, bottom - top);
            NewControlSpawnerPhantom.IsVisible = true;
        }
    }

    private void OnNewControlStarted(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(NewControlSpawner);
        if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            isSpawningNewControl = true;
            newControlInitialPress = point.Position;
        }
    }

    private void OnFormPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled)
            return;

        if (DataContext is not FormEditViewModel vm)
            return;

        vm.SelectedComponent = vm.Form;
    }

    private void VisualControl_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if ((e.Source as Control)?.DataContext is not ComponentInstanceViewModel vm)
            return;

        if (DataContext is not FormEditViewModel rootVm)
            return;

        var eventClass = vm.Instance.BaseClass.Events.FirstOrDefault();

        if (eventClass == null)
            rootVm.RequestCode(null);
        else
            rootVm.RequestCode($"{vm.Name}_{eventClass.Name}");
        e.Handled = true;
    }

    private void OnFormDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not FormEditViewModel rootVm)
            return;

        rootVm.RequestCode($"Form_Load");
        e.Handled = true;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        activateSub?.Dispose();
        if (DataContext is FormEditViewModel vm)
        {
            activateSub = vm.EventBus.Subscribe<ActivateFormEditorEvent>(form =>
            {
                if (form.Form == vm.FormDefinition)
                {
                    this.ActivateMDIForm();
                    form.Handled = true;
                }
            });
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        activateSub?.Dispose();
    }
}