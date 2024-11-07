using System.Threading.Tasks;
using AvaloniaVisualBasic.Runtime.Interpreter;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic.Runtime;

public class StandaloneStandardLib : IBasicStandardLibrary
{
    private readonly VBFormRuntime form;

    public StandaloneStandardLib(VBFormRuntime form)
    {
        this.form = form;
    }

    public async Task<MessageBoxResult> MsgBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return await MessageBox.ShowDialog(form, text, caption, buttons, icon);
    }

    public async Task<string?> InputBox(string prompt, string title, string defaultText)
    {
        return await Classic.CommonControls.Dialogs.InputBox.ShowDialog(form, prompt, title, defaultText);
    }
}