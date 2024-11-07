using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Utils;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Runtime.Serialization;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic.Projects;

public class ProjectService : IProjectService
{
    private readonly Func<NewProjectViewModel> newProjectVm;
    private readonly IWindowManager windowManager;
    private readonly IEventBus eventBus;
    private readonly IProjectManager projectManager;

    public ProjectService(Func<NewProjectViewModel> newProjectVm,
        IWindowManager windowManager,
        IEventBus eventBus,
        IProjectManager projectManager)
    {
        this.newProjectVm = newProjectVm;
        this.windowManager = windowManager;
        this.eventBus = eventBus;
        this.projectManager = projectManager;
    }

    private async Task<IProjectTemplate?> ChooseNewProject()
    {
        var vm = newProjectVm();
        if (!await windowManager.ShowDialog(vm) || vm.SelectedTemplate == null)
            return null;

        return vm.SelectedTemplate.Template;
    }

    public async Task CreateNewProject()
    {
        var template = await ChooseNewProject();
        if (template == null)
            return;

        var name = $"Project{projectManager.LoadedProjects.Count + 1}";

        projectManager.NewProject(template, name);
    }

    public async Task CreateNewProject(IProjectTemplate template)
    {
        if (template.Supported == false)
        {
            await windowManager.MessageBox("Your Avalonia Visual Basic version doesn't support " + template.Name, "Avalonia Visual Basic", MessageBoxButtons.Ok, MessageBoxIcon.Information);
            return;
        }

        var name = $"Project{projectManager.LoadedProjects.Count + 1}";

        projectManager.NewProject(template, name);
    }

    public async Task UnloadAllProjects()
    {
        if (projectManager.LoadedProjects.Count == 0)
            return;

        var changedFilesVm = new SaveChangesViewModel();
        foreach (var loadedProject in projectManager.LoadedProjects)
        {
            changedFilesVm.Add(loadedProject);
            foreach (var form in loadedProject.Forms)
                changedFilesVm.Add(form);
        }
        changedFilesVm.SelectedFiles.AddRange(changedFilesVm.ChangedFiles);
        if (!await windowManager.ShowDialog(changedFilesVm))
            throw new OperationCanceledException();

        if (changedFilesVm.SaveChanges)
        {
            foreach (var selected in changedFilesVm.SelectedFiles.Where(f => f.Form != null))
                await SaveForm(selected.Form!, false);

            foreach (var selected in changedFilesVm.SelectedFiles.Where(f => f.Project != null))
                await SaveOnlyProject(selected.Project!, false);
        }

        projectManager.UnloadAllProjects();
    }

    public async Task UnloadProject(ProjectDefinition project)
    {
        var changedFilesVm = new SaveChangesViewModel();
        changedFilesVm.Add(project);
        foreach (var form in project.Forms)
            changedFilesVm.Add(form);

        changedFilesVm.SelectedFiles.AddRange(changedFilesVm.ChangedFiles);
        if (!await windowManager.ShowDialog(changedFilesVm))
            throw new OperationCanceledException();

        if (changedFilesVm.SaveChanges)
        {
            foreach (var selected in changedFilesVm.SelectedFiles.Where(f => f.Form != null))
                await SaveForm(selected.Form!, false);

            foreach (var selected in changedFilesVm.SelectedFiles.Where(f => f.Project != null))
                await SaveOnlyProject(selected.Project!, false);
        }

        projectManager.UnloadProject(project);
    }

    public async Task OpenProject()
    {
        if (OperatingSystem.IsBrowser())
        {
            await windowManager.MessageBox("Opening projects is not supported in browser.", icon: MessageBoxIcon.Information);
            throw new OperationCanceledException();
        }

        await UnloadAllProjects();

        var files = await windowManager.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Project",
            FileTypeFilter =
            [
                new("Project Files") { Patterns = ["*.vbp"] },
                new("All Files") { Patterns = ["*.*"] }
            ],
            AllowMultiple = false
        });

        if (files == null || files.Count != 1)
            throw new OperationCanceledException();

        await OpenProject(files[0]);
    }

    private async Task OpenProject(string projectPath)
    {
        if (projectManager.LoadedProjects.Any(p => p.AbsolutePath != null && Path.GetFullPath(projectPath) == Path.GetFullPath(p.AbsolutePath)))
        {
            await windowManager.MessageBox($"Project {Path.GetFileName(projectPath)} is already opened.", icon: MessageBoxIcon.Information);
            throw new OperationCanceledException();
        }
        var projectDeserializer = new ProjectDeserializer();
        var formDeserializer = new FormDeserializer();
        var errorSink = new DeserializeErrorSink();

        var serializedProject = projectDeserializer.Deserialize(await File.ReadAllTextAsync(projectPath), errorSink);

        var project = new ProjectDefinition(serializedProject.ProjectType, serializedProject.Name ?? "Project1");
        project.AbsolutePath = projectPath;

        foreach (var formPath  in serializedProject.RelativeFormPaths)
        {
            var formAbsolutePath = Path.Join(Path.GetDirectoryName(projectPath)!, formPath);
            var form = formDeserializer.Deserialize(project, await File.ReadAllTextAsync(formAbsolutePath), errorSink);
            if (form != null)
            {
                form.AbsolutePath = formAbsolutePath;
                project.AddForm(form);
            }
        }

        if (serializedProject.StartupFormName != null)
            project.StartupForm = project.Forms.FirstOrDefault(x => x.Name == serializedProject.StartupFormName);

        projectManager.AddProject(project);

        if (errorSink.Errors.Count > 0)
        {
            await windowManager.MessageBox(
                "Couldn't properly deserialize the project:\n" + string.Join("\n", errorSink.Errors),
                icon: MessageBoxIcon.Warning);
        }
    }

    private class DeserializeErrorSink : IDeserializeErrorSink
    {
        private List<string> errors = new();

        public void LogError(string error)
        {
            errors.Add(error);
        }

        public IReadOnlyList<string> Errors => errors;
    }

    public async Task SaveForm(FormDefinition form, bool saveAs)
    {
        eventBus.Publish(new ApplyAllUnsavedChangesEvent());
        var formPath = form.AbsolutePath;
        if (formPath == null || saveAs)
        {
            formPath = await windowManager.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                DefaultExtension = "frm",
                SuggestedFileName = form.Name + ".frm",
                FileTypeChoices = [new("Form Files") { Patterns = ["*.frm"] }, new("All Files") { Patterns = ["*.*"] }]
            });
            if (formPath == null)
                throw new OperationCanceledException();
        }
        SerializeFormToFile(form, formPath);
    }

    public async Task MakeProject()
    {
        if (projectManager.StartupProject == null)
        {
            await windowManager.MessageBox("No startup project found.", icon: MessageBoxIcon.Information);
            return;
        }

        await MakeProject(projectManager.StartupProject);
    }

    public async Task EditProjectProperties(ProjectDefinition project)
    {
        var vm = new ProjectPropertiesViewModel(project);
        if (!await windowManager.ShowDialog(vm))
            return;

        vm.Apply(project);
    }

    public async Task EditProjectReferences(ProjectDefinition project)
    {
        var vm = new ReferencesViewModel(project);
        await windowManager.ShowDialog(vm);
    }

    public async Task EditProjectComponents(ProjectDefinition project)
    {
        var vm = new ComponentsViewModel(project);
        await windowManager.ShowDialog(vm);
    }

    public async Task MakeProject(ProjectDefinition projectDefinition)
    {
        if (OperatingSystem.IsBrowser())
        {
            await windowManager.MessageBox("Can't make project in a browser!", icon: MessageBoxIcon.Information);
            throw new OperationCanceledException();
        }

        var exePath = await windowManager.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            DefaultExtension = OperatingSystem.IsWindows() ? "exe" : "",
            SuggestedFileName = projectDefinition.Name + (OperatingSystem.IsWindows() ? ".exe" : ""),
            FileTypeChoices = OperatingSystem.IsWindows() ? [new("Windows EXE") { Patterns = ["*.exe"] }] : [new("All Files") { Patterns = ["*.*"] }]
        });

        if (exePath == null)
            throw new OperationCanceledException();

        List<FileInfo> requiredNativeFiles;

        if (OperatingSystem.IsWindows())
        {
            requiredNativeFiles =
            [
                new FileInfo("standalone/av_libglesv2.dll"),
                new FileInfo("standalone/AvaloniaVisualBasic.Standalone.exe"),
                new FileInfo("standalone/libHarfBuzzSharp.dll"),
                new FileInfo("standalone/libSkiaSharp.dll")
            ];
        }
        else if (OperatingSystem.IsMacOS())
        {
            requiredNativeFiles =
            [
                new FileInfo("standalone/libAvaloniaNative.dylib"),
                new FileInfo("standalone/AvaloniaVisualBasic.Standalone"),
                new FileInfo("standalone/libHarfBuzzSharp.dylib"),
                new FileInfo("standalone/libSkiaSharp.dylib")
            ];
        }
        else
        {
            await windowManager.MessageBox("Your OS is not supported yet, but it can be, search in the code for this message to find how to add support for this platform!", icon: MessageBoxIcon.Information);
            throw new OperationCanceledException();
        }

        if (requiredNativeFiles.Any(f => !f.Exists))
        {
            await windowManager.MessageBox("To Make Project, you need to build standalone runtime first. See the readme for help.", icon: MessageBoxIcon.Information);
            throw new OperationCanceledException();
        }

        var exeDir = Path.GetDirectoryName(exePath);
        var exeFileName = Path.GetFileNameWithoutExtension(exePath);

        var tempPath = Path.GetTempFileName();
        File.Delete(tempPath);
        Directory.CreateDirectory(tempPath);
        eventBus.Publish(new ApplyAllUnsavedChangesEvent());

        var originalAbsolutePath = projectDefinition.AbsolutePath;
        var originalFormPaths = projectDefinition.Forms.Select(x => (x, x.AbsolutePath)).ToList();

        foreach (var form in projectDefinition.Forms)
            SerializeFormToFile(form, Path.Join(tempPath, Path.ChangeExtension(form.Name, "frm")));
        SerializeOnlyProjectToFile(projectDefinition, Path.Join(tempPath, Path.ChangeExtension(exeFileName, "vbp")));

        // vvv this is a bad design. TODO a better way to handle paths
        projectDefinition.AbsolutePath = originalAbsolutePath;
        foreach (var (form, original) in originalFormPaths)
            form.AbsolutePath = original;

        // This is not a .dll file, but it will look better :wink: :wink:
        ZipFile.CreateFromDirectory(tempPath, Path.ChangeExtension(exePath, "dll")!);
        Directory.Delete(tempPath, true);

        foreach (var standaloneFile in requiredNativeFiles)
        {
            var fileName = standaloneFile.Name;
            if (fileName.StartsWith("AvaloniaVisualBasic.Standalone"))
                fileName = Path.GetFileName(exePath);
            standaloneFile.CopyTo(Path.Join(exeDir, fileName), true);
        }
    }

    public async Task SaveProject(ProjectDefinition project, bool saveAs)
    {
        eventBus.Publish(new ApplyAllUnsavedChangesEvent());
        foreach (var form in project.Forms)
        {
            await SaveForm(form, false);
        }

        await SaveOnlyProject(project, saveAs);
    }

    private async Task SaveOnlyProject(ProjectDefinition project, bool saveAs)
    {
        var projectPath = project.AbsolutePath;
        if (projectPath == null || saveAs)
        {
            projectPath = await windowManager.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                DefaultExtension = "vbp",
                SuggestedFileName = project.Name + ".vbp",
                FileTypeChoices = [new("Project Files") { Patterns = ["*.vbp"] }, new("All Files") { Patterns = ["*.*"] }]
            });
            if (projectPath == null)
                throw new OperationCanceledException();
        }
        SerializeOnlyProjectToFile(project, projectPath);
    }

    public async Task SaveAllProjects(bool saveAs)
    {
        foreach (var project in projectManager.LoadedProjects)
        {
            await SaveProject(project, saveAs);
        }
    }

    private void SerializeFormToFile(FormDefinition form, string formPath)
    {
        form.AbsolutePath = formPath;
        var serializer = new FormSerializer();
        var serialized = serializer.Serialize(form);

        File.WriteAllText(formPath, serialized);
    }

    private void SerializeOnlyProjectToFile(ProjectDefinition definition, string projectPath)
    {
        definition.AbsolutePath = projectPath;

        var serializer = new ProjectSerializer();
        var serialized = serializer.Serialize(definition, projectPath);

        File.WriteAllText(projectPath, serialized);
    }
}