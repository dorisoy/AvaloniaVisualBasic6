using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.IDE;

public interface IEditorService
{
    void EditForm(FormDefinition? form);
    void EditCode(FormDefinition? form);
}