using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Events;

public class ActivateFormEditorEvent : IEvent
{
    public ActivateFormEditorEvent(FormDefinition form)
    {
        Form = form;
    }

    public FormDefinition Form { get; }
    public bool Handled { get; set; }
}