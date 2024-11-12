namespace AvaloniaVisualBasic.Runtime.Components;

public class EventClass
{
    public EventClass(string name, params EventClassArgument[]? arguments)
    {
        Name = name;
        Arguments = arguments ?? [];
    }

    public string Name { get; }
    public EventClassArgument[] Arguments { get; }
}

public class EventClassArgument
{
    public EventClassArgument(string defaultName, string type)
    {
        DefaultName = defaultName;
        Type = type;
    }

    public string DefaultName { get; }
    public string Type { get; }
}