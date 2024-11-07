using System.Collections.Generic;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public class ExecutionState
{
    public Dictionary<int, Vb6Value> memory = new();
    private int nextFreeLocation = 0;

    public int Alloc(Vb6Value value)
    {
        var loc = nextFreeLocation++;
        memory[loc] = value;
        return loc;
    }

    public Vb6Value this[int location]
    {
        get => memory[location];
        set => memory[location] = value;
    }
}