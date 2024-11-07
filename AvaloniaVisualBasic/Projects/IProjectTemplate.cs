using Avalonia.Media.Imaging;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Projects;

public interface IProjectTemplate
{
    string Name { get; }
    Bitmap Icon { get; }
    bool Supported { get; }

    public static readonly IProjectTemplate StandardEXE = new ProjectTemplate("Standard EXE", "standard_exe", true, name =>
    {
        var project = new ProjectDefinition(VBProjectType.EXE, name);

        project.AddForm(new FormDefinition(project, "Form1"));

        return project;
    });

    public static readonly IProjectTemplate ActiveXEXE = new ProjectTemplate("ActiveX EXE", "activex_exe", false, _ => new ProjectDefinition(0, ""));
    public static readonly IProjectTemplate ActiveXDLL = new ProjectTemplate("ActiveX DLL", "activex_dll", false, _ => new ProjectDefinition(0, ""));
    public static readonly IProjectTemplate ActiveXControl = new ProjectTemplate("ActiveX Control", "activex_control", false, _ => new ProjectDefinition(0, ""));

    ProjectDefinition Create(string name);

    static readonly IProjectTemplate[] Templates =
    [
        StandardEXE,
        ActiveXEXE,
        ActiveXDLL,
        ActiveXControl,
        new ProjectTemplate("VB Application Wizard", "wizard", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("VB Wizard Manager", "wizard", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("ActiveX Document DLL", "generic", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("Addin", "generic", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("Data Project", "generic", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("DHTML Application", "generic", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("IIS Application", "generic", false, _ => new ProjectDefinition(0, "")),
        new ProjectTemplate("VB Enterprise Edition Controls", "generic", false, _ => new ProjectDefinition(0, "")),
    ];
}