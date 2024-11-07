using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBTimer : TemplatedControl
{
    public static readonly StyledProperty<int> IntervalProperty =
        AvaloniaProperty.Register<VBLabel, int>(nameof(Interval));

    public int Interval
    {
        get => GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    private IDisposable? timer;

    static VBTimer()
    {
        IntervalProperty.Changed.AddClassHandler<VBTimer>((timer, e) =>
        {
            timer.UpdateTimer();
        });
        IsEnabledProperty.Changed.AddClassHandler<VBTimer>((timer, e) =>
        {
            timer.UpdateTimer();
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UpdateTimer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timer?.Dispose();
        timer = null;
        if (IsEnabled && Interval > 0 && this.IsAttachedToVisualTree())
        {
            timer = DispatcherTimer.Run(() =>
            {
                this.ExecuteSub(TimerComponentClass.TimerEvent);
                return true;
            }, TimeSpan.FromMilliseconds(Interval));
        }
    }

    public IReadOnlyList<PropertyClass> AccessibleProperties { get; } =
        [VBProperties.EnabledProperty, VBProperties.IntervalProperty];

    public Vb6Value? GetPropertyValue(PropertyClass property)
    {
        if (property == VBProperties.EnabledProperty)
            return IsEnabled;
        if (property == VBProperties.IntervalProperty)
            return Interval;
        return null;
    }

    public void SetPropertyValue(PropertyClass property, Vb6Value value)
    {
        if (property == VBProperties.EnabledProperty)
            IsEnabled = value.Value is true;
        if (property == VBProperties.IntervalProperty)
            Interval = value.Value as int? ?? 0;
    }
}