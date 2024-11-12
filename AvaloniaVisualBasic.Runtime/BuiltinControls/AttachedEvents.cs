using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaVisualBasic.Runtime.Components;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public static class AttachedEvents
{
    public static void AttachFocusEvents<T>() where T : Control
    {
        InputElement.GotFocusEvent.AddClassHandler<T>((control, e) => control.ExecuteSub(ComponentBaseClass.GotFocusEvent));
        InputElement.LostFocusEvent.AddClassHandler<T>((control, e) => control.ExecuteSub(ComponentBaseClass.LostFocusEvent));
    }

    public static void AttachClick<T>() where T : Control
    {
        if (typeof(T).IsSubclassOf(typeof(Button)))
        {
            Button.ClickEvent.AddClassHandler<T>((control, e) => control.ExecuteSub(ComponentBaseClass.ClickEvent));
        }
        else
        {
            InputElement.PointerReleasedEvent.AddClassHandler<T>((control, e) => control.ExecuteSub(ComponentBaseClass.ClickEvent));
        }
    }
}