using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Avalonia;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Runtime.Serialization;
using Classic.CommonControls;

namespace AvaloniaVisualBasic.Standalone;

public class Program
{
    public static FormDefinition? StartupForm;
    public static string? Error;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            LoadProject(args);
        }
        catch (Exception e)
        {
            Error = e.Message;
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void LoadProject(string[] args)
    {
        string? projectPath = null;

        if (args.Length == 0)
        {
            if (Environment.ProcessPath == null)
            {
                Error = $"Couldn't extract current process path.";
            }
            else
            {
                var dir = Path.GetDirectoryName(Environment.ProcessPath);
                var fileName = Path.GetFileNameWithoutExtension(Environment.ProcessPath);
                projectPath = Path.Join(dir, Path.ChangeExtension(fileName, "dll"));
            }
        }
        else
        {
            projectPath = args[0];
        }

        if (projectPath == null || !File.Exists(projectPath))
        {
            Error = $"Couldn't find any project in {projectPath}";
        }
        else
        {
            var projectDeserializer = new ProjectDeserializer();
            var formDeserializer = new FormDeserializer();

            // This is not a .dll file, but it will look better :wink: :wink:
            if (string.Equals(Path.GetExtension(projectPath), ".dll", StringComparison.OrdinalIgnoreCase))
            {
                var tempPath = Path.GetTempFileName();
                File.Delete(tempPath);
                ZipFile.ExtractToDirectory(projectPath, tempPath);

                var actualProjectPath = Path.Join(tempPath,
                    Path.ChangeExtension(Path.GetFileNameWithoutExtension(projectPath), "vbp"));

                if (!File.Exists(actualProjectPath))
                {
                    Error = $"{projectPath} is not a valid project";
                    return;
                }

                projectPath = actualProjectPath;
            }

            var serializedProject = projectDeserializer.Deserialize(File.ReadAllText(projectPath), new NullErrorSink());
            var forms = new List<FormDefinition>();

            var projectDefinition = new ProjectDefinition(serializedProject.ProjectType, serializedProject.Name ?? "Unnamed project");

            foreach (var relativeFormPath in serializedProject.RelativeFormPaths)
            {
                var absolutePath = Path.Join(Path.GetDirectoryName(projectPath)!, relativeFormPath);
                var form = formDeserializer.Deserialize(projectDefinition, File.ReadAllText(absolutePath), new NullErrorSink());
                if (form != null)
                    forms.Add(form);
            }

            var startupForm = forms.FirstOrDefault(x => x.Name == serializedProject.StartupFormName);

            if (startupForm == null)
            {
                Error = "No startup form found";
                return;
            }

            StartupForm = startupForm;
        }
    }

    private class NullErrorSink : IDeserializeErrorSink
    {
        public void LogError(string error)
        {
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseMessageBoxSounds()
            .LogToTrace();
}