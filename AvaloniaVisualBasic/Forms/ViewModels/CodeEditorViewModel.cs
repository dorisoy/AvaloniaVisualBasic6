using System;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaEdit.Document;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Runtime.Interpreter;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Utils;
using Classic.CommonControls.Dialogs;
using PropertyChanged.SourceGenerator;
using R3;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class CodeEditorViewModel : BaseEditorWindowViewModel
{
    private readonly IWindowManager windowManager;
    private readonly IEditorService editorService;
    private readonly IProjectService projectService;
    private readonly IEventBus eventBus;
    public override string Title => $"{formDefinition?.Owner.Name} - {formDefinition?.Name} (Code)";
    public override IImage Icon { get; } = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaVisualBasic/Icons/codeeditor.gif")));

    private TextDocument document = new TextDocument();
    private FormDefinition? formDefinition;

    public TextDocument Document => document;

    [Notify] private int caretOffset;

    public event Action? FocusWindowRequest;

    public IEventBus EventBus => eventBus;

    public FormDefinition? FormDefinition => formDefinition;

    SyntaxChecker syntaxChecker = new SyntaxChecker();

    public CodeEditorViewModel(IWindowManager windowManager,
        IEditorService editorService,
        IProjectService projectService,
        IEventBus eventBus)
    {
        this.windowManager = windowManager;
        this.editorService = editorService;
        this.projectService = projectService;
        this.eventBus = eventBus;

        AutoDispose(this.eventBus.Subscribe<CreateOrNavigateToSubEvent>(e =>
        {
            if (e.Form == formDefinition)
            {
                var sub = Document.IndexOf($"Sub {e.Sub}", 0, Document.TextLength, StringComparison.OrdinalIgnoreCase);
                if (sub != -1)
                {
                    var nextNewLineIndex = Document.IndexOf("\n", sub, Document.TextLength - sub, StringComparison.OrdinalIgnoreCase);
                    CaretOffset = nextNewLineIndex == -1 ? sub : nextNewLineIndex + 1;
                }
                else
                {
                    AddProcedureViewModel vm = new AddProcedureViewModel();
                    vm.IsPublic = true;
                    vm.IsSub = true;
                    vm.Name = e.Sub;
                    var code = vm.GenerateCode();
                    InsertAtEnd(code.beginCode, code.endCode);
                }
                FocusWindowRequest?.Invoke();
            }
        }));
        AutoDispose(this.eventBus.Subscribe<ApplyAllUnsavedChangesEvent>(e =>
        {
            formDefinition.UpdateCode(Document.Text);
        }));
        AutoDispose(this.eventBus.Subscribe<FormUnloadedEvent>(e =>
        {
            if (e.Form == formDefinition)
                RequestClose();
        }));
        AutoDispose(new ActionDisposable(() => formDefinition.UpdateCode(Document.Text)));
    }

    public CodeEditorViewModel Initialize(FormDefinition formElement)
    {
        AutoDispose(formElement.ObservePropertyChanged(x => x.Name)
            .Subscribe(name => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)))));
        AutoDispose(formElement.Owner.ObservePropertyChanged(x => x.Name)
            .Subscribe(name => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)))));
        this.formDefinition = formElement;
        Document.Text = formElement.Code;
        return this;
    }

    public void CheckSyntax(int onlyLine)
    {
        try
        {
            syntaxChecker.Run(Document.Text);
        }
        catch (VBCompileErrorException error)
        {
            if (error.Line == onlyLine)
                windowManager.MessageBox(error.Message, icon: MessageBoxIcon.Warning).ListenErrors();
        }
    }

    public void SaveForm() => projectService.SaveForm(formDefinition, false).ListenErrors();

    public void SaveFormAs() => projectService.SaveForm(formDefinition, true).ListenErrors();

    public void ViewCode() => editorService.EditCode(formDefinition);

    public void ViewObject() => editorService.EditForm(formDefinition);

    public async Task AddProcedure()
    {
        var vm = new AddProcedureViewModel();
        if (!await windowManager.ShowDialog(vm))
            return;

        var code = vm.GenerateCode();
        InsertAtEnd(code.beginCode, code.endCode);
    }

    private void InsertAtEnd(string beginCode, string endCode)
    {
        var textLen = Document.TextLength;
        if (textLen >= 2)
        {
            var end = Document.GetText(textLen - 2, 2);
            if (end != "\n\n")
                Document.Insert(textLen, "\n\n");
            else if (end[1] == '\n')
                Document.Insert(textLen, "\n");
        }

        Document.Insert(Document.TextLength, beginCode);
        var offset = Document.TextLength;
        Document.Insert(Document.TextLength, endCode);
        CaretOffset = offset;
        FocusWindowRequest?.Invoke();
    }
}