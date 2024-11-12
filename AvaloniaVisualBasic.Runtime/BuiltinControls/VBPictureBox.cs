using Avalonia.Controls.Primitives;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBPictureBox : TemplatedControl
{
    static VBPictureBox()
    {
        AttachedEvents.AttachClick<VBPictureBox>();
    }
}