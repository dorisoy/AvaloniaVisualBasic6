namespace AvaloniaVisualBasic.Runtime.Interpreter;

#pragma warning disable CS1998
public enum ControlFlow
{
    Nothing,
    ExitDo,
    ExitFor,
    ExitFunction,
    ExitProperty,
    ExitSub,
    ContinueDo
}