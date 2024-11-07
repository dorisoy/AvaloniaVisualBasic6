using Dock.Model.Mvvm.Controls;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Utils;

public partial class EditorToolBase : Tool
{
    [Notify] private bool isActive;
}