using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public class ProjectStartupObjectViewModel
{
    public string Header { get; }

    public FormDefinition? Form { get; }

    public ProjectStartupObjectViewModel(FormDefinition form)
    {
        Form = form;
        Header = form.Name;
    }

    public override string ToString() => Header;
}