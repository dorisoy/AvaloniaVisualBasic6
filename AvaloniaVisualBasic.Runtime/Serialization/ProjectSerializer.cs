using System;
using System.IO;
using System.Text;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using Path = System.IO.Path;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class ProjectSerializer
{
    private static string ProjectTypeToString(VBProjectType type)
    {
        return type switch
        {
            VBProjectType.EXE => "EXE",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public string Serialize(ProjectDefinition project, string projectPath)
    {
        StringWriter writer = new ();
        writer.NewLine = "\r\n";

        writer.WriteLine($"{SerializedProject.NameKey}={project.Name}");
        writer.WriteLine($"{SerializedProject.TypeKey}={ProjectTypeToString(project.ProjectType)}");
        foreach (var form in project.Forms)
        {
            if (form.AbsolutePath == null)
                continue;
            var relativePath = Path.GetRelativePath(Path.GetDirectoryName(projectPath)!, form.AbsolutePath);
            writer.WriteLine($"{SerializedProject.FormKey}={relativePath}");
        }

        if (project.StartupForm != null)
        {
            writer.WriteLine($"{SerializedProject.StartupKey}={project.StartupForm.Name}");
        }

        return writer.ToString();
    }
}