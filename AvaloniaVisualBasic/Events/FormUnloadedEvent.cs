using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Events;

public class FormUnloadedEvent : IEvent
{
    public FormDefinition Form { get; }

    public FormUnloadedEvent(FormDefinition form)
    {
        Form = form;
    }
}