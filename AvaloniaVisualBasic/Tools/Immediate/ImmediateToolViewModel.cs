using AvaloniaEdit.Document;
using AvaloniaVisualBasic.Utils;
using Dock.Model.Mvvm.Controls;

namespace AvaloniaVisualBasic.Tools;

public partial class ImmediateToolViewModel : EditorToolBase
{
    public TextDocument Document { get; } = new();

    public ImmediateToolViewModel()
    {
        Title = "Immediate";
        CanPin = false;
        CanClose = true;
    }
}