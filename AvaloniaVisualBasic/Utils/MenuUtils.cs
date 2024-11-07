using Avalonia;
using Avalonia.Controls;
using Avalonia.Labs.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Classic.CommonControls;

namespace AvaloniaVisualBasic.Utils;

public class MenuUtils
{
    public static readonly AttachedProperty<Bitmap> MenuIconProperty = AvaloniaProperty.RegisterAttached<MenuUtils, MenuItem, Bitmap>("MenuIcon");

    public static readonly AttachedProperty<RoutedCommand> CommandProperty = AvaloniaProperty.RegisterAttached<MenuUtils, MenuItem, RoutedCommand>("Command");

    public static Bitmap GetMenuIcon(AvaloniaObject element) => element.GetValue(MenuIconProperty);

    public static void SetMenuIcon(AvaloniaObject element, Bitmap value) => element.SetValue(MenuIconProperty, value);

    public static RoutedCommand GetCommand(AvaloniaObject element) => element.GetValue(CommandProperty);

    public static void SetCommand(AvaloniaObject element, RoutedCommand value) => element.SetValue(CommandProperty, value);

    static MenuUtils()
    {
        MenuIconProperty.Changed.AddClassHandler<MenuItem>((menuItem, e) =>
        {
            menuItem.Icon = new IconRenderer()
            {
                Source = e.NewValue as Bitmap
            };
        });
        MenuItem.CommandProperty.Changed.AddClassHandler<MenuItem>((menuItem, e) =>
        {
            if (e.NewValue as RoutedCommand is {} routedCommand)
            {
                if (routedCommand.Gestures.Count >= 1)
                    menuItem.InputGesture = routedCommand.Gestures[0];
            }
        });
    }
}