using System.Collections.Generic;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public class ExecutionEnvironment
{
    public Dictionary<string, int> variableToLocation = new();

    private ExecutionEnvironment(ExecutionEnvironment other)
    {
        foreach (var (name, loc) in other.variableToLocation)
            variableToLocation[name] = loc;
    }

    public ExecutionEnvironment()
    {

    }

    public void DefineVariable(string name, int location)
    {
        variableToLocation[name] = location;
    }

    public bool TryGetVariableLocation(string name, out int location)
    {
        return variableToLocation.TryGetValue(name, out location);
    }

    public ExecutionEnvironment Clone()
    {
        return new ExecutionEnvironment(this);
    }
}