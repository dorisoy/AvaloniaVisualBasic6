using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Labs.Input;
using Avalonia.Platform;
using AvaloniaVisualBasic.Controls;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.Runtime;
using AvaloniaVisualBasic.Runtime.BuiltinControls;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Utils;
using AvaloniaVisualBasic.VisualDesigner;
using Classic.CommonControls.Dialogs;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.IDE;

public partial class ProjectRunnerService : IProjectRunnerService
{
    private readonly IEventBus eventBus;
    private readonly IWindowManager windowManager;
    private readonly IProjectManager projectManager;

    [Notify]
    [AlsoNotify(nameof(IsRunning), nameof(CanStartDefaultProject), nameof(CanStartDefaultProjectWithFullCompile), nameof(CanBreakProject), nameof(CanEndProject), nameof(CanRestartProject))]
    private System.IDisposable? runningProject;

    public bool IsRunning => runningProject != null;

    public ProjectRunnerService(IEventBus eventBus,
        IWindowManager windowManager,
        IProjectManager projectManager)
    {
        this.eventBus = eventBus;
        this.windowManager = windowManager;
        this.projectManager = projectManager;
    }

    private void OnIsRunningChanged() => CommandManager.InvalidateRequerySuggested();

    public void RunProject(ProjectDefinition projectDefinition)
    {
        eventBus.Publish(new ApplyAllUnsavedChangesEvent());

        if (projectDefinition.StartupForm is { } form)
        {
            async Task WindowTask()
            {
                var tokenSource = new CancellationTokenSource();
                if (Static.SingleView)
                {
                    var task = RunFormInBrowser(form, tokenSource.Token, out var window);
                    RunningProject = new ActionDisposable(() => tokenSource.Cancel());
                    await task;
                    RunningProject = null;
                }
                else
                {
                    var task = VBLoader.RunForm(form, tokenSource.Token, out var window);
                    window.Show();
                    RunningProject = new ActionDisposable(() => tokenSource.Cancel());
                    await task;
                    RunningProject = null;
                }
            }
            WindowTask().ListenErrors();
        }
        else
        {
            windowManager.MessageBox("Must have a startup form or Sub Main()", icon: MessageBoxIcon.Error);
        }
    }

    public void RunStartupProject()
    {
        if (projectManager.StartupProject is {} startupProject)
        {
            RunProject(startupProject);
        }
    }

    public void BreakCurrentProject()
    {
        throw new NotImplementedException();
    }

    public void EndProject()
    {
        RunningProject?.Dispose();
        RunningProject = null;
    }

    public void RestartProject()
    {
        EndProject();
        RunStartupProject();
    }

    public bool CanStartDefaultProject => !IsRunning && projectManager.StartupProject != null;
    public bool CanStartDefaultProjectWithFullCompile => CanStartDefaultProject;
    public bool CanBreakProject => IsRunning && false;
    public bool CanEndProject => IsRunning;
    public bool CanRestartProject => IsRunning;

    private VBMDIFormRuntime InstantiateWindow(ComponentInstance instance)
    {
        var form = new VBMDIFormRuntime(windowManager)
        {
            Title = instance.GetPropertyOrDefault(VBProperties.CaptionProperty) ?? "",
            Width = instance.GetPropertyOrDefault(VBProperties.WidthProperty),
            Height = instance.GetPropertyOrDefault(VBProperties.HeightProperty),
            [AttachedProperties.BackColorProperty] = instance.GetPropertyOrDefault(VBProperties.BackColorProperty),
            [MDIHostPanel.WindowLocationProperty] = new Point((int)instance.GetPropertyOrDefault(VBProperties.LeftProperty), (int)instance.GetPropertyOrDefault(VBProperties.TopProperty))
        };
        VBProps.SetName(form, instance.GetPropertyOrDefault(VBProperties.NameProperty));
        return form;
    }
    
    private Task RunFormInBrowser(FormDefinition element, CancellationToken token, out VBMDIFormRuntime window)
    {
        var form = element.Components.FirstOrDefault(x => x.BaseClass == FormComponentClass.Instance);
        if (form == null)
            throw new Exception("No form found");

        window = InstantiateWindow(form);
        if (form.GetPropertyOrDefault(VBProperties.NameProperty) is { } formName)
            window.Context.ExecutionContext.AllocVariable(window.Context.RootEnv, formName, new Vb6Value(window));

        var task = windowManager.ShowManagedWindow(window);
        window.Content = VBLoader.SpawnComponents(element, window.Context.ExecutionContext, window.Context.RootEnv);

        window.Context.SetCode(code: element.Code);
        token.Register((state, _) =>
        {
            (state as MDIWindow)!.CloseCommand.Execute(null);
        }, window);

        return task;
    }

    public class VBMDIFormRuntime : MDIWindow, IModuleExecutionRoot
    {
        protected override Type StyleKeyOverride => typeof(MDIWindow);

        private VBWindowContext windowContext;

        public VBWindowContext Context => windowContext;

        public VBMDIFormRuntime(IWindowManager windowManager)
        {
            windowContext = new VBWindowContext(new MDIStandaloneStandardLib(windowManager));
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            windowContext.ExecuteSub("Form_Load");
        }

        public void ExecuteSub(string name)
        {
            windowContext.ExecuteSub(name);
        }
    }

    public class MDIStandaloneStandardLib : IBasicStandardLibrary
    {
        private readonly IWindowManager windowManager;

        public MDIStandaloneStandardLib(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public async Task<MessageBoxResult> MsgBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return await windowManager.MessageBox(text, caption, buttons, icon);
        }

        public async Task<string?> InputBox(string prompt, string title, string defaultText)
        {
            return await windowManager.InputBox(prompt, title, defaultText);
        }
    }
}