using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaVisualBasic.Controls;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Runtime;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;
using AvaloniaVisualBasic.Tools;
using AvaloniaVisualBasic.Utils;
using AvaloniaVisualBasic.VisualDesigner;
using Classic.CommonControls.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using PropertyChanged.SourceGenerator;
using R3;
using MdiWindowManager = AvaloniaVisualBasic.IDE.MdiWindowManager;

namespace AvaloniaVisualBasic;

public partial class MainViewViewModel : ObservableObject
{
    private readonly IWindowManager windowManager;
    private readonly IProjectService projectService;
    private readonly DockFactory dockFactory;
    private readonly IProjectRunnerService projectRunnerService;
    private readonly IEventBus eventBus;

    public IMdiWindowManager MdiWindowManager { get; }

    public ToolBoxToolViewModel ToolBox { get; }

    public PropertiesToolViewModel Properties { get; }
    public ImmediateToolViewModel Immediate { get; }
    public FormLayoutToolViewModel FormLayout { get; }
    public LocalsToolViewModel Locals { get; }
    public WatchesToolViewModel Watches { get; }
    public ProjectToolViewModel ProjectExplorer { get; }
    public ColorPaletteToolViewModel ColorPalette { get; }
    public IFocusedProjectUtil FocusedProjectUtil { get; }

    public IRootDock Layout { get; }

    public DelegateCommand StartDefaultProjectCommand { get; }

    public DelegateCommand StartDefaultProjectWithFullCompileCommand { get; }

    public DelegateCommand BreakProjectCommand { get; }

    public DelegateCommand EndProjectCommand { get; }

    public DelegateCommand RestartProjectCommand { get; }

    public DelegateCommand ProjectReferencesCommand { get; }

    public DelegateCommand ProjectComponentsCommand { get; }

    public DelegateCommand ProjectPropertiesCommand { get; }

    public string Title => FocusedProjectUtil.FocusedOrStartupProject == null
        ? "Avalonia Visual Basic [design]"
        : $"{FocusedProjectUtil.FocusedOrStartupProject.Name} - Avalonia Visual Basic " + (projectRunnerService.IsRunning ? "[run]" : "[design]");

    [Notify] private bool isStandardToolbarVisible = true;

    public class DockFactory : Factory
    {
        private readonly IMdiWindowManager mdiWindowManager;
        private readonly ToolBoxToolViewModel toolBox;
        private readonly ProjectToolViewModel project;
        private readonly PropertiesToolViewModel properties;
        private readonly FormLayoutToolViewModel formLayout;
        private readonly MDIControllerViewModel mdiController;

        public ProportionalDock? RightDock;
        public ProportionalDock? MiddleDock;

        public DockFactory(IMdiWindowManager mdiWindowManager,
            ToolBoxToolViewModel toolBox,
            ProjectToolViewModel project,
            PropertiesToolViewModel properties,
            FormLayoutToolViewModel formLayout,
            MDIControllerViewModel mdiController)
        {
            this.mdiWindowManager = mdiWindowManager;
            this.toolBox = toolBox;
            this.project = project;
            this.properties = properties;
            this.formLayout = formLayout;
            this.mdiController = mdiController;
        }

        public override IToolDock CreateToolDock()
        {
            var toolDock = base.CreateToolDock();
            toolDock.CanFloat = false;
            return toolDock;
        }

        public override IRootDock CreateLayout()
        {
            var toolBoxTool = CreateToolDock();
            toolBoxTool.ActiveDockable = toolBox;
            toolBoxTool.VisibleDockables = CreateList<IDockable>(toolBox);
            toolBoxTool.Alignment = Alignment.Left;
            toolBoxTool.Proportion = 0.06;

            var projectTool = CreateToolDock();
            projectTool.ActiveDockable = project;
            projectTool.VisibleDockables = CreateList<IDockable>(project);
            projectTool.Alignment = Alignment.Top;

            var propertiesTool = CreateToolDock();
            propertiesTool.ActiveDockable = properties;
            propertiesTool.VisibleDockables = CreateList<IDockable>(properties);
            propertiesTool.Alignment = Alignment.Top;

            var formLayoutTool = CreateToolDock();
            formLayoutTool.ActiveDockable = formLayout;
            formLayoutTool.VisibleDockables = CreateList<IDockable>(formLayout);
            formLayoutTool.Alignment = Alignment.Top;

            RightDock = new ProportionalDock
            {
                Orientation = Orientation.Vertical,
                Proportion = 0.2155,
                CanClose = false,
                CanFloat = false,
                IsCollapsable = false,
                Context = nameof(RightDock),
                VisibleDockables = CreateList<IDockable>
                (
                    projectTool,
                    new ProportionalDockSplitter(),
                    propertiesTool,
                    new ProportionalDockSplitter(),
                    formLayoutTool
                )
            };

            var documentDock = new DocumentDock()
            {
                ActiveDockable = mdiController,
                CanFloat = false,
                VisibleDockables = CreateList<IDockable>(mdiController),
            };

            MiddleDock = new ProportionalDock()
            {
                Orientation = Orientation.Vertical,
                CanClose = false,
                IsCollapsable = false,
                CanFloat = false,
                Context = nameof(MiddleDock),
                VisibleDockables = CreateList<IDockable>
                (
                    documentDock
                )
            };

            var mainLayout = new ProportionalDock
            {
                Orientation = Orientation.Horizontal,
                CanFloat = false,
                VisibleDockables = CreateList<IDockable>
                (
                    toolBoxTool,
                    new ProportionalDockSplitter(),
                    MiddleDock,
                    new ProportionalDockSplitter(),
                    RightDock
                )
            };

            var rootDock = CreateRootDock();

            rootDock.IsFocusableRoot = true;
            rootDock.IsCollapsable = false;
            rootDock.ActiveDockable = mainLayout;
            rootDock.DefaultDockable = mainLayout;
            rootDock.VisibleDockables = CreateList<IDockable>(mainLayout);

            return rootDock;
        }
    }

    public MainViewViewModel(IWindowManager windowManager,
        MdiWindowManager mdiWindowManager,
        ToolBoxToolViewModel toolBox,
        PropertiesToolViewModel properties,
        ImmediateToolViewModel immediate,
        FormLayoutToolViewModel formLayout,
        LocalsToolViewModel locals,
        WatchesToolViewModel watches,
        ProjectToolViewModel projectExplorer,
        ColorPaletteToolViewModel colorPalette,
        IProjectManager projectManager,
        IFocusedProjectUtil focusedProjectUtil,
        IProjectService projectService,
        DockFactory dockFactory,
        IProjectRunnerService projectRunnerService,
        IEventBus eventBus)
    {
        this.windowManager = windowManager;
        this.projectService = projectService;
        this.dockFactory = dockFactory;
        this.projectRunnerService = projectRunnerService;
        this.eventBus = eventBus;
        MdiWindowManager = mdiWindowManager;
        ToolBox = toolBox;
        Properties = properties;
        Immediate = immediate;
        FormLayout = formLayout;
        Locals = locals;
        Watches = watches;
        ProjectExplorer = projectExplorer;
        ColorPalette = colorPalette;
        FocusedProjectUtil = focusedProjectUtil;

        Layout = dockFactory.CreateLayout();
        dockFactory.InitLayout(Layout);

        VBWindowContext.RunTimeError += (form, e) =>
        {
            var line = form.Code.Substring(e.Context.Start.StartIndex, e.Context.Stop.StopIndex - e.Context.Start.StartIndex);
            var vm = new RuntimeErrorViewModel(e.Message + "\n\nat " + line);
            windowManager.ShowDialog(vm);
        };

        VBWindowContext.CompileError += (form, e) =>
        {
            //var line = form.Code.Substring(e.Context.Start.StartIndex, e.Context.Stop.StopIndex - e.Context.Start.StartIndex);
            //var vm = new RuntimeErrorViewModel(e.Message + "\n\nat " + line);
            windowManager.MessageBox(e.Message, "Avalonia Visual Basic", MessageBoxButtons.Ok, MessageBoxIcon.Warning);
        };

        StartDefaultProjectCommand = new DelegateCommand(projectRunnerService.RunStartupProject,
            () => projectRunnerService.CanStartDefaultProject);
        StartDefaultProjectWithFullCompileCommand = new DelegateCommand(projectRunnerService.RunStartupProject,
            () => projectRunnerService.CanStartDefaultProjectWithFullCompile);
        BreakProjectCommand = new DelegateCommand(projectRunnerService.BreakCurrentProject,
            () => projectRunnerService.CanBreakProject);
        EndProjectCommand = new DelegateCommand(projectRunnerService.EndProject,
            () => projectRunnerService.CanEndProject);
        RestartProjectCommand = new DelegateCommand(projectRunnerService.RestartProject,
            () => projectRunnerService.CanRestartProject);
        ProjectReferencesCommand = new DelegateCommand(() => projectService.EditProjectReferences(FocusedProjectUtil.FocusedOrStartupProject!),
            () => FocusedProjectUtil.FocusedOrStartupProject != null);
        ProjectComponentsCommand = new DelegateCommand(() => projectService.EditProjectComponents(FocusedProjectUtil.FocusedOrStartupProject!),
            () => FocusedProjectUtil.FocusedOrStartupProject != null);
        ProjectPropertiesCommand = new DelegateCommand(() => projectService.EditProjectProperties(FocusedProjectUtil.FocusedOrStartupProject!),
            () => FocusedProjectUtil.FocusedOrStartupProject != null);
        MakeProjectCommand = new DelegateCommand(() => projectService.MakeProject(FocusedProjectUtil.FocusedOrStartupProject!).ListenErrors(),
            () => FocusedProjectUtil.FocusedOrStartupProject != null);
        RemoveProjectCommand = new DelegateCommand(() => projectService.UnloadProject(FocusedProjectUtil.FocusedOrStartupProject!).ListenErrors(),
            () => FocusedProjectUtil.FocusedOrStartupProject != null);

        FocusedProjectUtil.ObservePropertyChanged(x => x.FocusedOrStartupProject)
            .Subscribe(_ =>
            {
                ProjectReferencesCommand.RaiseCanExecutedChanged();
                ProjectComponentsCommand.RaiseCanExecutedChanged();
                ProjectPropertiesCommand.RaiseCanExecutedChanged();
                MakeProjectCommand.RaiseCanExecutedChanged();
                RemoveProjectCommand.RaiseCanExecutedChanged();
                OnPropertyChanged(nameof(Title));
            });

        projectRunnerService.ObservePropertyChanged(x => x.IsRunning)
            .Subscribe(_ => OnPropertyChanged(nameof(Title)));
    }

    public void OnInitialized()
    {
        projectService.CreateNewProject().ListenErrors();
    }

    public void NYI()
    {
        MessageBox.ShowDialog((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow, "This feature is not yet implemented.", "NYI", MessageBoxButtons.Ok, MessageBoxIcon.Information);
    }

    public void SaveProject() => projectService.SaveAllProjects(false).ListenErrors();

    public void SaveProjectAs() => projectService.SaveAllProjects(true).ListenErrors();

    public void OpenProject() => projectService.OpenProject().ListenErrors();

    public DelegateCommand MakeProjectCommand { get; }

    public DelegateCommand RemoveProjectCommand { get; }

    public void MakeProject() => projectService.MakeProject().ListenErrors();

    public void OpenAddInManager() => windowManager.ShowDialog(new AddInManagerViewModel());

    public void OpenOptions() => windowManager.ShowDialog(new OptionsViewModel());

    public async Task NewProject()
    {
        await projectService.UnloadAllProjects();
        await projectService.CreateNewProject();
    }

    public async Task AddProject(IProjectTemplate? projectTemplate)
    {
        if (projectTemplate == null)
            await projectService.CreateNewProject();
        else
            await projectService.CreateNewProject(projectTemplate);
    }

    private void DebuggingNotImplementedYet() =>
        windowManager.MessageBox("Debugging is not yet implemented. Maybe one day?", icon: MessageBoxIcon.Information).ListenErrors();

    public void StepInto() => DebuggingNotImplementedYet();
    public void StepOver() => DebuggingNotImplementedYet();
    public void StepOut() => DebuggingNotImplementedYet();
    public void RunToCursor() => DebuggingNotImplementedYet();
    public void AddWatch() => DebuggingNotImplementedYet();
    public void EditWatch() => DebuggingNotImplementedYet();
    public void QuickWatch() => DebuggingNotImplementedYet();
    public void ToggleBreakpoint() => DebuggingNotImplementedYet();
    public void ClearAllBreakpoints() => DebuggingNotImplementedYet();
    public void SetNextStatement() => DebuggingNotImplementedYet();
    public void ShowNextStatement() => DebuggingNotImplementedYet();

    public async Task OpenGithubRepo()
    {
        if (await windowManager.MessageBox(
                "This will open a new tab with this project github repo, but due to a bug in .NET/Avalonia it will also freeze this tab (just refresh the tab).",
                buttons: MessageBoxButtons.YesNo) == MessageBoxResult.No)
            return;

        TopLevel.GetTopLevel(Static.MainView).Launcher.LaunchUriAsync(new Uri("https://github.com/BAndysc/AvaloniaVisualBasic6"));
    }

    public void TileHorizontally() => eventBus.Publish(new RearrangeMDIEvent(MDIRearrangeKind.TileHorizontally));

    public void TileVertically() => eventBus.Publish(new RearrangeMDIEvent(MDIRearrangeKind.TileVertically));

    public void Cascade() => eventBus.Publish(new RearrangeMDIEvent(MDIRearrangeKind.Cascade));

    private T? FindDock<T>(Func<T, bool> action) where T : class, IDockable => FindDock<T>(Layout, action);

    private T? FindDock<T>(IDockable dockable, Func<T, bool> predicate) where T : class, IDockable
    {
        if (dockable is T t && predicate(t))
            return t;
        if (dockable is IDock dock && dock.VisibleDockables != null)
        {
            foreach (var visible in dock.VisibleDockables)
                if (FindDock<T>(visible, predicate) is { } ret)
                    return ret;
        }

        return null;
    }

    private void OpenOrActivateTool(Tool tool, bool right)
    {
        var opened = FindDock<IDockable>(x => ReferenceEquals(x, tool));
        if (opened != null)
        {
            dockFactory.SetFocusedDockable(opened.Owner as IDock, opened);
            return;
        }

        var middle = FindDock<IDock>(x => x.Context?.Equals(right ? nameof(DockFactory.RightDock) : nameof(DockFactory.MiddleDock)) ?? false);
        var toolDock = dockFactory.CreateToolDock();
        toolDock.ActiveDockable = tool;
        toolDock.Factory = dockFactory;
        toolDock.Proportion = 0.3;
        toolDock.VisibleDockables = dockFactory.CreateList<IDockable>(tool);
        toolDock.Alignment = right ? Alignment.Right : Alignment.Bottom;
        middle.VisibleDockables.Add(new ProportionalDockSplitter());
        middle.VisibleDockables.Add(toolDock);
        dockFactory.InitDockable(toolDock, middle);
        dockFactory.SetFocusedDockable(toolDock, opened);
    }

    public void OpenImmediateTool() => OpenOrActivateTool(Immediate, false);
    public void OpenLocalsTool() => OpenOrActivateTool(Locals, false);
    public void OpenWatchesTool() => OpenOrActivateTool(Watches, false);
    public void OpenColorPaletteTool() => OpenOrActivateTool(ColorPalette, false);
    public void OpenProjectExplorerTool() => OpenOrActivateTool(ProjectExplorer, true);
    public void OpenPropertiesTool() => OpenOrActivateTool(Properties, true);
    public void OpenFormLayoutTool() => OpenOrActivateTool(FormLayout, true);
    public void OpenToolBox() => OpenOrActivateTool(ToolBox, true);
}

public class VBMDIWindow : MDIWindow, IModuleExecutionRoot
{
    protected override Type StyleKeyOverride => typeof(MDIWindow);

    public IReadOnlyList<PropertyClass> AccessibleProperties { get; } = [VBProperties.CaptionProperty];

    private BasicInterpreter? interpreter;

    public ModuleExecutionContext ExecutionContext { get; } = new();

    public ExecutionEnvironment Environment { get; } = new();

    public void SetCode(string code)
    {
        interpreter = new BasicInterpreter(new StandaloneStandardLib(this), ExecutionContext, Environment, code);
    }

    public void ExecuteSub(string name)
    {
        interpreter!.ExecuteSub(name, null, true);
    }

    public Vb6Value? GetPropertyValue(PropertyClass property)
    {
        if (property == VBProperties.CaptionProperty)
            return Title;
        return null;
    }

    public void SetPropertyValue(PropertyClass property, Vb6Value value)
    {
        if (property == VBProperties.CaptionProperty)
            Title = value.Value?.ToString() ?? "";
    }

    public class StandaloneStandardLib : IBasicStandardLibrary
    {
        private readonly VBMDIWindow form;

        public StandaloneStandardLib(VBMDIWindow form)
        {
            this.form = form;
        }

        public async Task<MessageBoxResult> MsgBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return default;
            //return await MessageBox.ShowDialog(form, text, caption, buttons, icon);
        }

        public async Task<string?> InputBox(string prompt, string title, string defaultText)
        {
            return default;
        }
    }
}
