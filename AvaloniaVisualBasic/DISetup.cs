using System;
using Pure.DI;
using System.Diagnostics;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Tools;
using AvaloniaVisualBasic.VisualDesigner;
using static Pure.DI.Lifetime;
using MdiWindowManager = AvaloniaVisualBasic.IDE.MdiWindowManager;

namespace AvaloniaVisualBasic;

public partial class DISetup
{
    [Conditional("DI")]
    static void Setup() =>
        DI.Setup()
            .Bind().As(Singleton).To<ToolBoxToolViewModel>()
            .Bind().As(Singleton).To<PropertiesToolViewModel>()
            .Bind().As(Singleton).To<ProjectToolViewModel>()
            .Bind().As(Singleton).To<FormLayoutToolViewModel>()
            .Bind().As(Singleton).To<ImmediateToolViewModel>()
            .Bind().As(Singleton).To<LocalsToolViewModel>()
            .Bind().As(Singleton).To<WatchesToolViewModel>()
            .Bind().As(Singleton).To<ColorPaletteToolViewModel>()
            .Bind().As(Singleton).To<MdiWindowManager>()
            .Bind().As(Singleton).To<WindowManager>()
            .Bind().As(Singleton).To<ProjectManager>()
            .Bind().As(Singleton).To<EditorService>()
            .Bind().As(Singleton).To<MainViewViewModel.DockFactory>()
            .Bind().As(Singleton).To<EventBus>()
            .Bind().As(Singleton).To<ProjectRunnerService>()
            .Bind().As(Singleton).To<ProjectService>()
            .Bind().As(Singleton).To<FocusedProjectUtil>()
            .Root<MainViewViewModel>("Root");

    public static MainViewViewModel DesignTimeRootViewModel => new DISetup().Root;
}