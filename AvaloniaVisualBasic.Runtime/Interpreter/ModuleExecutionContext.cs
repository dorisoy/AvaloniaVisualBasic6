namespace AvaloniaVisualBasic.Runtime.Interpreter;

public class ModuleExecutionContext
{
    private ExecutionState state = new();

    public ExecutionState State => state;

    public bool TryUpdateVariable(ExecutionEnvironment env, string name, Vb6Value value)
    {
        if (env.TryGetVariableLocation(name, out var loc))
        {
            state[loc] = value;
            return true;
        }

        return false;
    }

    public bool TryGetVariable(ExecutionEnvironment env, string name, out Vb6Value value)
    {
        if (env.TryGetVariableLocation(name, out var loc))
        {
            value = state[loc];
            return true;
        }
        value = default;
        return false;
    }

    public void AllocVariable(ExecutionEnvironment env, string name, Vb6Value value)
    {
        var loc = state.Alloc(value);
        env.DefineVariable(name, loc);
    }
}