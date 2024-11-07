using System.Collections.Generic;
using Avalonia.Controls;
using static AvaloniaVisualBasic.Runtime.Components.VBProperties;

namespace AvaloniaVisualBasic.Runtime.Components;

public class MenuComponentClass : ComponentBaseClass
{
    public MenuComponentClass() : base([CaptionProperty, EnabledProperty, CheckedProperty, WindowListProperty])
    {
    }

    public override string Name => "Menu";
    public override string VBTypeName => "VB.Menu";

    protected override Control InstantiateInternal(ComponentInstance instance)
    {
        return new MenuItem()
        {
            Header = instance.GetPropertyOrDefault(CaptionProperty),
            IsEnabled = instance.GetPropertyOrDefault(EnabledProperty),
            IsChecked = instance.GetPropertyOrDefault(CheckedProperty),
            ToggleType = instance.GetPropertyOrDefault(CheckedProperty) ? MenuItemToggleType.CheckBox : MenuItemToggleType.None,
            IsVisible = instance.GetPropertyOrDefault(VisibleProperty)
        };
    }

    public static PropertyClass<List<ComponentInstance>?> SubItemsProperty = new PropertyClass<List<ComponentInstance>?>("SubItems", "", PropertyCategory.Internal);

    public static ComponentBaseClass Instance { get; } = new MenuComponentClass();
}