using System;
using Avalonia.Controls.Primitives;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBFrame : HeaderedContentControl
{
    protected override Type StyleKeyOverride => typeof(HeaderedContentControl);

    static VBFrame()
    {
        AttachedEvents.AttachClick<VBFrame>();
    }
}