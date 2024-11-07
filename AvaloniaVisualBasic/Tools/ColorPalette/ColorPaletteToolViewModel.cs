using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.VisualDesigner;
using Dock.Model.Mvvm.Controls;
using PropertyChanged.SourceGenerator;
using R3;

namespace AvaloniaVisualBasic.Tools;

public partial class ColorPaletteToolViewModel : Tool
{
    private readonly IMdiWindowManager mdiWindowManager;

    private System.IDisposable? activeWindowSub;

    private ComponentInstance? boundComponentInstance;

    private bool ignoreNotifications;

    public ColorPaletteToolViewModel(IMdiWindowManager mdiWindowManager)
    {
        this.mdiWindowManager = mdiWindowManager;
        selectedBackColor = new ColorPaletteViewModel(this, Colors.Transparent);
        selectedForeColor = new ColorPaletteViewModel(this, Colors.Transparent);
        Title = "";
        CanPin = false;
        CanClose = true;

        Palette = [
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xC0, 0xC0),
            new(this, 0xFF, 0xE0, 0xC2),
            new(this, 0xFE, 0xFF, 0xC4),
            new(this, 0xB8, 0xFF, 0xC4),
            new(this, 0xB9, 0xFF, 0xFF),
            new(this, 0xC1, 0xC0, 0xFC),
            new(this, 0xFF, 0xC0, 0xFC),
            new(this, 0xE0, 0xE0, 0xE0),
            new(this, 0xFF, 0x7F, 0x81),
            new(this, 0xFF, 0xBF, 0x86),
            new(this, 0xFE, 0xFF, 0x8C),
            new(this, 0x66, 0xFF, 0x8A),
            new(this, 0x6A, 0xFF, 0xFE),
            new(this, 0x83, 0x81, 0xFA),
            new(this, 0xFF, 0x80, 0xFB),
            new(this, 0xC0, 0xC0, 0xC0),
            new(this, 0xFF, 0x00, 0x14),
            new(this, 0xFF, 0x7F, 0x24),
            new(this, 0xFD, 0xFE, 0x43),
            new(this, 0x00, 0xFF, 0x3F),
            new(this, 0x00, 0xFF, 0xFE),
            new(this, 0x20, 0x12, 0xF9),
            new(this, 0xFF, 0x06, 0xF9),
            new(this, 0x80, 0x80, 0x80),
            new(this, 0xC8, 0x00, 0x0C),
            new(this, 0xC7, 0x3F, 0x12),
            new(this, 0xBF, 0xBF, 0x30),
            new(this, 0x00, 0xC0, 0x2D),
            new(this, 0x00, 0xC0, 0xBF),
            new(this, 0x15, 0x0B, 0xBB),
            new(this, 0xC9, 0x03, 0xBC),
            new(this, 0x40, 0x40, 0x40),
            new(this, 0x85, 0x00, 0x05),
            new(this, 0x84, 0x3F, 0x0D),
            new(this, 0x7F, 0x80, 0x1D),
            new(this, 0x00, 0x80, 0x1B),
            new(this, 0x00, 0x80, 0x80),
            new(this, 0x0A, 0x04, 0x7D),
            new(this, 0x86, 0x01, 0x7D),
            new(this, 0x00, 0x00, 0x00),
            new(this, 0x43, 0x00, 0x01),
            new(this, 0x84, 0x40, 0x41),
            new(this, 0x40, 0x40, 0x0A),
            new(this, 0x00, 0x40, 0x08),
            new(this, 0x00, 0x40, 0x40),
            new(this, 0x02, 0x01, 0x3E),
            new(this, 0x43, 0x00, 0x3E),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF),
            new(this, 0xFF, 0xFF, 0xFF)
        ];

        mdiWindowManager.ObservePropertyChanged(x => x.ActiveWindow)
            .Subscribe(window =>
            {
                if (boundComponentInstance != null)
                {
                    boundComponentInstance.OnComponentPropertyChanged -= OnComponentPropertyChanged;
                    boundComponentInstance = null;
                }
                activeWindowSub?.Dispose();
                activeWindowSub = null;

                if (window is not FormEditViewModel vm)
                    return;

                activeWindowSub = vm.ObservePropertyChanged(x => x.SelectedComponent)
                    .Subscribe(component =>
                    {
                        if (boundComponentInstance != null)
                        {
                            boundComponentInstance.OnComponentPropertyChanged -= OnComponentPropertyChanged;
                            boundComponentInstance = null;
                        }
                        if (component == null)
                            return;

                        boundComponentInstance = component.Instance;
                        boundComponentInstance.OnComponentPropertyChanged += OnComponentPropertyChanged;

                        OnComponentPropertyChanged(boundComponentInstance, VBProperties.BackColorProperty);
                        OnComponentPropertyChanged(boundComponentInstance, VBProperties.ForeColorProperty);
                    });
            });
    }

    private void OnComponentPropertyChanged(ComponentInstance component, PropertyClass propertyClass)
    {
        if (ignoreNotifications)
            return;

        if (propertyClass == VBProperties.BackColorProperty &&
            component.BaseClass.Properties.Contains(VBProperties.BackColorProperty))
        {
            var color = component.GetPropertyOrDefault(VBProperties.BackColorProperty);
            selectedBackColor = new ColorPaletteViewModel(this, color);
            OnPropertyChanged(nameof(SelectedBackColor));
            OnPropertyChanged(nameof(SelectedColor));
        }
        else if (propertyClass == VBProperties.ForeColorProperty &&
            component.BaseClass.Properties.Contains(VBProperties.ForeColorProperty))
        {
            var color = component.GetPropertyOrDefault(VBProperties.ForeColorProperty);
            selectedForeColor = new ColorPaletteViewModel(this, color);
            OnPropertyChanged(nameof(SelectedForeColor));
            OnPropertyChanged(nameof(SelectedColor));
        }
    }

    public void OnSelectedColorChanged()
    {
        if (SelectedColor == null)
            return;

        if (mdiWindowManager.ActiveWindow is not FormEditViewModel formEdit)
            return;

        if (formEdit.SelectedComponent is not { } selectedComponent)
            return;

        var prop = PickingBackColor ? VBProperties.BackColorProperty : VBProperties.ForeColorProperty;
        if (!selectedComponent.Instance.BaseClass.Properties.Contains(prop))
            return;

        ignoreNotifications = true;
        selectedComponent.Instance.SetProperty(prop, SelectedColor.Color);
        ignoreNotifications = false;
    }

    public ObservableCollection<ColorPaletteViewModel> Palette { get; }

    [Notify] [AlsoNotify(nameof(SelectedColor))] private ColorPaletteViewModel? selectedBackColor;
    [Notify] [AlsoNotify(nameof(SelectedColor))] private ColorPaletteViewModel? selectedForeColor;
    [Notify] [AlsoNotify(nameof(SelectedColor))] private bool pickingBackColor = true;

    public ColorPaletteViewModel? SelectedColor
    {
        get => pickingBackColor ? selectedBackColor : selectedForeColor;
        set
        {
            if (value == null)
                return;

            if (pickingBackColor)
                SelectedBackColor = value;
            else
                SelectedForeColor = value;
            OnSelectedColorChanged();
        }
    }
}

public partial class ColorPaletteViewModel
{
    private readonly ColorPaletteToolViewModel vm;

    public ColorPaletteViewModel(ColorPaletteToolViewModel vm, VBColor color)
    {
        this.vm = vm;
        this.color = color;
    }

    public ColorPaletteViewModel(ColorPaletteToolViewModel vm, Color color)
    {
        this.vm = vm;
        this.color = VBColor.FromColor(color);
    }

    public ColorPaletteViewModel(ColorPaletteToolViewModel vm, byte r, byte g, byte b)
    {
        this.vm = vm;
        color = VBColor.FromColor(r, g, b);
    }

    [Notify] private VBColor color;

    private void OnColorChanged()
    {
        vm.OnSelectedColorChanged();
    }
}