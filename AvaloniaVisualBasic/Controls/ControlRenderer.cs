using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.VisualDesigner;
using ComponentInstanceViewModel = AvaloniaVisualBasic.VisualDesigner.ComponentInstanceViewModel;

namespace AvaloniaVisualBasic.Controls;

public class ControlRenderer : Decorator
{
    public static readonly StyledProperty<ComponentInstanceViewModel?> ComponentProperty = AvaloniaProperty.Register<ControlRenderer, ComponentInstanceViewModel?>("Component");

    public ComponentInstanceViewModel? Component
    {
        get => GetValue(ComponentProperty);
        set => SetValue(ComponentProperty, value);
    }

    static ControlRenderer()
    {
        ComponentProperty.Changed.AddClassHandler<ControlRenderer>(((renderer, args) => renderer.UpdateContent()));
        BoundsProperty.Changed.AddClassHandler<ControlRenderer>((renderer, args) => renderer.UpdateContent());
    }

    private ComponentInstanceViewModel? boundViewModel;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Detach();
        if (Component != null)
        {
            boundViewModel = Component;
            Component.Instance.OnComponentPropertyChanged += ComponentOnOnComponentPropertyChanged;
        }
    }

    private void Detach()
    {
        if (boundViewModel != null)
        {
            boundViewModel.Instance.OnComponentPropertyChanged -= ComponentOnOnComponentPropertyChanged;
            boundViewModel = null;
        }
    }

    private void ComponentOnOnComponentPropertyChanged(ComponentInstance instance, PropertyClass property)
    {
        if (property == VBProperties.LeftProperty ||
            property == VBProperties.TopProperty ||
            property == VBProperties.WidthProperty ||
            property == VBProperties.HeightProperty)
            return;
        UpdateContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Detach();
    }

    private void UpdateContent()
    {
        Child = null;
        if (Component != null && Bounds.Width >= 1 && Bounds.Width >= 1)
        {
            RenderTargetBitmap target = new RenderTargetBitmap(new PixelSize((int)Bounds.Width, (int)Bounds.Height),
                new Vector(96, 96));
            var visual = Component.Instance.BaseClass.Instantiate(Component.Instance);
            VisualChildren.Add(visual);
            LogicalChildren.Add(visual);

            visual.Width = target.PixelSize.Width;
            visual.Height = target.PixelSize.Height;
            visual.Measure(new Size(target.PixelSize.Width, target.PixelSize.Height));
            visual.Arrange(new Rect(0, 0, target.PixelSize.Width, target.PixelSize.Height));

            this.UpdateLayout();

            MinHeight = visual.MinHeight;
            MinWidth = visual.MinWidth;
            MaxWidth = visual.MaxWidth;
            MaxHeight = visual.MaxHeight;

            RenderOptions.SetTextRenderingMode(visual, TextRenderingMode.Alias);
            target.Render(visual);
            LogicalChildren.Remove(visual);
            VisualChildren.Remove(visual);
            Child = new Image()
            {
                Source = target
            };
        }
    }
}