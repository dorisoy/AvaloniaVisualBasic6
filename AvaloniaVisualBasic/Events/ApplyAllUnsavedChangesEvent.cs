using AvaloniaVisualBasic.IDE;

namespace AvaloniaVisualBasic.Events;

// this is pretty bad design, but editors lazily save back to FormDefinition now. TODO
public class ApplyAllUnsavedChangesEvent : IEvent
{

}