using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Labs.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using AvaloniaVisualBasic.Controls;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.Utils;
using Dock.Model.Mvvm.Controls;
using R3;

namespace AvaloniaVisualBasic.Forms.Views;

public partial class CodeEditorView : UserControl
{
    private IDisposable? sub, activateSub;

    public DelegateCommand Undo { get; }
    public DelegateCommand Redo { get; }
    public DelegateCommand Copy { get; }
    public DelegateCommand Cut { get; }
    public DelegateCommand Delete { get; }
    public DelegateCommand Paste { get; }
    public DelegateCommand Find { get; }
    public DelegateCommand Replace { get; }
    public DelegateCommand SelectAll { get; }

    static CodeEditorView()
    {
        var uri = new Uri("avares://AvaloniaVisualBasic/Resources/TextHighlighting/VB6.xshd.xml");

        // Get the asset loader from Avalonia
        var xshdContent = AssetLoader.Open(uri);
        
        if (xshdContent == null)
            throw new InvalidOperationException("VB6 XSHD resource not found");

        using var reader = new XmlTextReader(xshdContent);
        var xshd = HighlightingLoader.LoadXshd(reader);
        var highlightingDefinition = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
        HighlightingManager.Instance.RegisterHighlighting("VB6", new[] { ".vb6", ".bas", ".frm", ".cls" }, highlightingDefinition);
    }
    
    public CodeEditorView()
    {
        Undo = new DelegateCommand(() => TextEditor.Undo(), () => TextEditor?.CanUndo ?? false);
        Redo = new DelegateCommand(() => TextEditor.Redo(), () =>
            TextEditor?.CanRedo ?? false);
        Copy = new DelegateCommand(() => TextEditor.Copy(), () => true);
        Cut = new DelegateCommand(() => TextEditor.Cut(), () => true);
        Delete = new DelegateCommand(() => TextEditor.Delete(), () => true);
        Paste = new DelegateCommand(() => TextEditor.Paste(), () => true);
        SelectAll = new DelegateCommand(() => TextEditor.SelectAll(), () => true);
        Find = new DelegateCommand(() =>
        {
            AvaloniaEdit.ApplicationCommands.Find.Execute(null, TextEditor.TextArea);
        }, () => true);
        Replace = new DelegateCommand(() =>
        {
            AvaloniaEdit.ApplicationCommands.Replace.Execute(null, TextEditor.TextArea);
        }, () => true);

        InitializeComponent();

        TextEditor.TextChanged += TextChanged;

        TextEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
    }

    private void TextChanged(object? sender, EventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
    }

    public void Indent()
    {
        EditingCommands.TabForward.Execute(null, TextEditor.TextArea);
    }

    public void Outdent()
    {
        EditingCommands.TabBackward.Execute(null, TextEditor.TextArea);
    }

    public async Task InsertFile()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var fileContent = await streamReader.ReadToEndAsync();
            TextEditor.SelectedText = fileContent;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        sub?.Dispose();
        activateSub?.Dispose();
        sub = null;
        if (DataContext is CodeEditorViewModel vm)
        {
            sub = vm.ObservePropertyChanged(x => x.CaretOffset)
                .Subscribe(offset =>
                {
                    if (TextEditor.CaretOffset != offset)
                        TextEditor.CaretOffset = offset;
                });
            vm.FocusWindowRequest += VmOnFocusWindowRequest;

            activateSub = vm.EventBus.Subscribe<ActivateCodeEditorEvent>(form =>
            {
                if (form.Form == vm.FormDefinition)
                {
                    this.ActivateMDIForm();
                    form.Handled = true;
                }
            });

            TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("VB6");
        }
    }

    private void VmOnFocusWindowRequest()
    {
        this.ActivateMDIForm();
        TextEditor.Focus();
        TextEditor.ScrollToLine(TextEditor.TextArea.Caret.Line);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (DataContext is CodeEditorViewModel vm)
        {
            vm.FocusWindowRequest -= VmOnFocusWindowRequest;
        }

        sub?.Dispose();
        sub = null;

        activateSub?.Dispose();
    }

    private void OnCaretPositionChanged(object? sender, EventArgs e)
    {
        if (DataContext is CodeEditorViewModel vm)
        {
            vm.CaretOffset = TextEditor.CaretOffset;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desiredSize = base.MeasureOverride(availableSize);

        return new Size(Math.Clamp(700, desiredSize.Width, availableSize.Width),
            Math.Clamp(380, desiredSize.Height, availableSize.Height));
    }
}