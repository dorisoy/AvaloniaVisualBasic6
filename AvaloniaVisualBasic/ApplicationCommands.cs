using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Labs.Input;

namespace AvaloniaVisualBasic;

public class ApplicationCommands
{
    private static readonly KeyModifiers PlatformCommandKey = GetPlatformCommandKey();

    public static readonly RoutedCommand NewProjectCommand = new RoutedCommand(nameof(NewProjectCommand), new KeyGesture(Key.N, GetPlatformCommandKey()));
    public static readonly RoutedCommand AddProjectCommand = new RoutedCommand(nameof(AddProjectCommand));
    public static readonly RoutedCommand OpenProjectCommand = new RoutedCommand(nameof(OpenProjectCommand), new KeyGesture(Key.O, GetPlatformCommandKey()));
    public static readonly RoutedCommand EditMenuCommand = new RoutedCommand(nameof(EditMenuCommand), new KeyGesture(Key.E, GetPlatformCommandKey()));
    public static readonly RoutedCommand NYICommand = new RoutedCommand(nameof(NYICommand));
    public static readonly RoutedCommand AboutCommand = new RoutedCommand(nameof(AboutCommand));
    public static readonly RoutedCommand AvaloniaOnWebCommand = new RoutedCommand(nameof(AvaloniaOnWebCommand));
    public static readonly RoutedCommand AddInManagerCommand = new RoutedCommand(nameof(AddInManagerCommand));
    public static readonly RoutedCommand AddProcedureCommand = new RoutedCommand(nameof(AddProcedureCommand));
    public static readonly RoutedCommand OpenOptionsCommand = new RoutedCommand(nameof(OpenOptionsCommand));
    public static readonly RoutedCommand SaveProjectCommand = new RoutedCommand(nameof(SaveProjectCommand));
    public static readonly RoutedCommand SaveProjectAsCommand = new RoutedCommand(nameof(SaveProjectAsCommand));
    public static readonly RoutedCommand SaveCommand = new RoutedCommand(nameof(SaveCommand), new KeyGesture(Key.S, GetPlatformCommandKey()));
    public static readonly RoutedCommand SaveAsCommand = new RoutedCommand(nameof(SaveAsCommand));
    public static readonly RoutedCommand ViewCodeCommand = new RoutedCommand(nameof(ViewCodeCommand));
    public static readonly RoutedCommand ViewObjectCommand = new RoutedCommand(nameof(ViewObjectCommand), new KeyGesture(Key.F7, KeyModifiers.Shift));
    public static readonly RoutedCommand OpenImmediateCommand = new RoutedCommand(nameof(OpenImmediateCommand), new KeyGesture(Key.G, GetPlatformCommandKey()));
    public static readonly RoutedCommand OpenLocalsCommand = new RoutedCommand(nameof(OpenLocalsCommand));
    public static readonly RoutedCommand OpenWatchesCommand = new RoutedCommand(nameof(OpenWatchesCommand));
    public static readonly RoutedCommand OpenProjectExplorerCommand = new RoutedCommand(nameof(OpenProjectExplorerCommand), new KeyGesture(Key.R, GetPlatformCommandKey()));
    public static readonly RoutedCommand OpenPropertiesCommand = new RoutedCommand(nameof(OpenPropertiesCommand), new KeyGesture(Key.F4));
    public static readonly RoutedCommand OpenFormLayoutCommand = new RoutedCommand(nameof(OpenFormLayoutCommand));
    public static readonly RoutedCommand OpenObjectBrowserCommand = new RoutedCommand(nameof(OpenObjectBrowserCommand));
    public static readonly RoutedCommand OpenToolBoxCommand = new RoutedCommand(nameof(OpenToolBoxCommand));
    public static readonly RoutedCommand OpenDataViewCommand = new RoutedCommand(nameof(OpenDataViewCommand));
    public static readonly RoutedCommand OpenVisualComponentCommand = new RoutedCommand(nameof(OpenVisualComponentCommand));
    public static readonly RoutedCommand OpenColorPaletteCommand = new RoutedCommand(nameof(OpenColorPaletteCommand));
    public static readonly RoutedCommand StartDefaultProjectCommand = new RoutedCommand(nameof(StartDefaultProjectCommand), new KeyGesture(Key.F5));
    public static readonly RoutedCommand StartDefaultProjectWithFullCompileCommand = new RoutedCommand(nameof(StartDefaultProjectWithFullCompileCommand), new KeyGesture(Key.F5, GetPlatformCommandKey()));
    public static readonly RoutedCommand BreakProjectCommand = new RoutedCommand(nameof(BreakProjectCommand), new KeyGesture(Key.Pause, GetPlatformCommandKey()));
    public static readonly RoutedCommand EndProjectCommand = new RoutedCommand(nameof(EndProjectCommand));
    public static readonly RoutedCommand RestartProjectCommand = new RoutedCommand(nameof(RestartProjectCommand), new KeyGesture(Key.F5, KeyModifiers.Shift));
    public static readonly RoutedCommand UndoCommand = new RoutedCommand(nameof(UndoCommand), new KeyGesture(Key.Z, GetPlatformCommandKey()));
    public static readonly RoutedCommand RedoCommand = new RoutedCommand(nameof(RedoCommand), new KeyGesture(Key.Z, GetPlatformCommandKey() | KeyModifiers.Shift));
    public static readonly RoutedCommand CutCommand = new RoutedCommand(nameof(CutCommand), new KeyGesture(Key.X, GetPlatformCommandKey()));
    public static readonly RoutedCommand CopyCommand = new RoutedCommand(nameof(CopyCommand), new KeyGesture(Key.C, GetPlatformCommandKey()));
    public static readonly RoutedCommand PasteCommand = new RoutedCommand(nameof(PasteCommand), new KeyGesture(Key.V, GetPlatformCommandKey()));
    public static readonly RoutedCommand PasteLinkCommand = new RoutedCommand(nameof(PasteLinkCommand));
    public static readonly RoutedCommand RemoveCommand = new RoutedCommand(nameof(RemoveCommand));
    public static readonly RoutedCommand DeleteCommand = new RoutedCommand(nameof(DeleteCommand), new KeyGesture(Key.Delete));
    public static readonly RoutedCommand DeleteTableFromDatabaseCommand = new RoutedCommand(nameof(DeleteTableFromDatabaseCommand));
    public static readonly RoutedCommand SelectAllCommand = new RoutedCommand(nameof(SelectAllCommand), new KeyGesture(Key.A, GetPlatformCommandKey()));
    public static readonly RoutedCommand SelectAllColumnsCommand = new RoutedCommand(nameof(SelectAllColumnsCommand));
    public static readonly RoutedCommand FindCommand = new RoutedCommand(nameof(FindCommand), new KeyGesture(Key.F, GetPlatformCommandKey()));
    public static readonly RoutedCommand FindNextCommand = new RoutedCommand(nameof(FindNextCommand), new KeyGesture(Key.F3));
    public static readonly RoutedCommand ReplaceCommand = new RoutedCommand(nameof(ReplaceCommand), new KeyGesture(Key.R, GetPlatformCommandKey()));
    public static readonly RoutedCommand IndentCommand = new RoutedCommand(nameof(IndentCommand), new KeyGesture(Key.Tab, GetPlatformCommandKey()));
    public static readonly RoutedCommand OutdentCommand = new RoutedCommand(nameof(OutdentCommand), new KeyGesture(Key.Tab, KeyModifiers.Shift));
    public static readonly RoutedCommand InsertFileCommand = new RoutedCommand(nameof(InsertFileCommand));
    public static readonly RoutedCommand ListPropertiesMethodsCommand = new RoutedCommand(nameof(ListPropertiesMethodsCommand), new KeyGesture(Key.J, GetPlatformCommandKey()));
    public static readonly RoutedCommand ListConstantsCommand = new RoutedCommand(nameof(ListConstantsCommand), new KeyGesture(Key.J, GetPlatformCommandKey() | KeyModifiers.Shift));
    public static readonly RoutedCommand QuickInfoCommand = new RoutedCommand(nameof(QuickInfoCommand), new KeyGesture(Key.I, GetPlatformCommandKey()));
    public static readonly RoutedCommand ParameterInfoCommand = new RoutedCommand(nameof(ParameterInfoCommand), new KeyGesture(Key.I, GetPlatformCommandKey() | KeyModifiers.Shift));
    public static readonly RoutedCommand CompleteWordCommand = new RoutedCommand(nameof(CompleteWordCommand), new KeyGesture(Key.Space, GetPlatformCommandKey()));
    public static readonly RoutedCommand BookmarksCommand = new RoutedCommand(nameof(BookmarksCommand));
    public static readonly RoutedCommand BringToFrontCommand = new RoutedCommand(nameof(BringToFrontCommand), new KeyGesture(Key.J, GetPlatformCommandKey()));
    public static readonly RoutedCommand SendToBackCommand = new RoutedCommand(nameof(SendToBackCommand), new KeyGesture(Key.K, GetPlatformCommandKey()));
    public static readonly RoutedCommand MakeProjectCommand = new RoutedCommand(nameof(MakeProjectCommand));
    public static readonly RoutedCommand MakeProjectGroupCommand = new RoutedCommand(nameof(MakeProjectGroupCommand));
    public static readonly RoutedCommand RemoveProjectCommand = new RoutedCommand(nameof(RemoveProjectCommand));
    public static readonly RoutedCommand ProjectReferencesCommand = new RoutedCommand(nameof(ProjectReferencesCommand));
    public static readonly RoutedCommand ProjectComponentsCommand = new RoutedCommand(nameof(ProjectComponentsCommand));
    public static readonly RoutedCommand ProjectPropertiesCommand = new RoutedCommand(nameof(ProjectPropertiesCommand));

    public static readonly RoutedCommand StepIntoCommand = new RoutedCommand(nameof(StepIntoCommand));
    public static readonly RoutedCommand StepOverCommand = new RoutedCommand(nameof(StepOverCommand));
    public static readonly RoutedCommand StepOutCommand = new RoutedCommand(nameof(StepOutCommand));
    public static readonly RoutedCommand RunToCursorCommand = new RoutedCommand(nameof(RunToCursorCommand));
    public static readonly RoutedCommand AddWatchCommand = new RoutedCommand(nameof(AddWatchCommand));
    public static readonly RoutedCommand EditWatchCommand = new RoutedCommand(nameof(EditWatchCommand));
    public static readonly RoutedCommand QuickWatchCommand = new RoutedCommand(nameof(QuickWatchCommand));
    public static readonly RoutedCommand ToggleBreakpointCommand = new RoutedCommand(nameof(ToggleBreakpointCommand));
    public static readonly RoutedCommand ClearAllBreakpointsCommand = new RoutedCommand(nameof(ClearAllBreakpointsCommand));
    public static readonly RoutedCommand SetNextStatementCommand = new RoutedCommand(nameof(SetNextStatementCommand));
    public static readonly RoutedCommand ShowNextStatementCommand = new RoutedCommand(nameof(ShowNextStatementCommand));

    public static readonly RoutedCommand CenterHorizontallyCommand = new RoutedCommand(nameof(CenterHorizontallyCommand));
    public static readonly RoutedCommand CenterVerticallyCommand = new RoutedCommand(nameof(CenterVerticallyCommand));

    public static readonly ICommand DisabledCommand = new BaseDisabledCommand();

    private class BaseDisabledCommand : ICommand
    {
        public bool CanExecute(object? parameter) => false;
        public void Execute(object? parameter) { }
        public event EventHandler? CanExecuteChanged;
    }

    private static KeyModifiers GetPlatformCommandKey()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return KeyModifiers.Meta;
        }

        return KeyModifiers.Control;
    }

    private static KeyGesture GetReplaceKeyGesture()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new KeyGesture(Key.F, KeyModifiers.Meta | KeyModifiers.Alt);
        }

        return new KeyGesture(Key.H, PlatformCommandKey);
    }
}