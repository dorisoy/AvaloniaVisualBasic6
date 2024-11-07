using AvaloniaVisualBasic.IDE;

namespace AvaloniaVisualBasic.Events;

public class RearrangeMDIEvent : IEvent
{
    public RearrangeMDIEvent(MDIRearrangeKind kind)
    {
        Kind = kind;
    }

    public MDIRearrangeKind Kind { get; }
}

public enum MDIRearrangeKind
{
    TileHorizontally,
    TileVertically,
    Cascade
}