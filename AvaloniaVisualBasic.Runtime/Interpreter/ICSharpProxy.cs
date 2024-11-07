using System.Collections.Generic;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public interface ICSharpProxy
{
    void Call(string method, List<Vb6Value> args);
}