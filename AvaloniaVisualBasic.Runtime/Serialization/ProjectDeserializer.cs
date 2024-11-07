using System;
using System.Collections.Generic;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class ProjectDeserializer
{
    private static VBProjectType ProjectTypeFromString(string type)
    {
        if (type.Equals("exe", StringComparison.OrdinalIgnoreCase))
            return VBProjectType.EXE;

        throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }

    public SerializedProject Deserialize(string source, IDeserializeErrorSink errorSink)
    {
        SerializedProject project = new();
        foreach (var line in source.Split('\n'))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 0)
                continue;
            var key = parts[0].Trim();
            var value = parts.Length == 2 ? parts[1].Trim() : "";
            if (value.StartsWith('"') && value.EndsWith('"'))
                value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            if (key.Equals(SerializedProject.NameKey, StringComparison.OrdinalIgnoreCase))
            {
                project.Name = value;
            }
            else if (key.Equals(SerializedProject.TypeKey, StringComparison.OrdinalIgnoreCase))
            {
                project.ProjectType = ProjectTypeFromString(value);
            }
            else if (key.Equals(SerializedProject.FormKey, StringComparison.OrdinalIgnoreCase))
            {
                project.RelativeFormPaths.Add(value);
            }
            else if (key.Equals(SerializedProject.StartupKey, StringComparison.OrdinalIgnoreCase))
            {
                project.StartupFormName = value;
            }
        }

        return project;
    }
}

public class SerializedProject
{
    public VBProjectType ProjectType { get; set; }
    public string? Name { get; set; }
    public List<string> RelativeFormPaths { get; } = new();
    public string? StartupFormName { get; set; }

    public const string NameKey = "Name";
    public const string TypeKey = "Type";
    public const string FormKey = "Form";
    public const string StartupKey = "Startup";
}