using AvaloniaVisualBasic.Runtime.Interpreter;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic.Runtime.Tests;

public abstract class BaseVBTestFixture
{
    protected List<Vb6Value> debug = new();
    protected  ModuleExecutionContext context;
    protected ExecutionEnvironment rootEnv;

    [SetUp]
    public void Setup()
    {
        context = new ModuleExecutionContext();
        rootEnv = new ExecutionEnvironment();
        var debugProxy = new DebugProxy(debug);
        context.AllocVariable(rootEnv, "Debug", new Vb6Value(debugProxy));
        debug.Clear();
    }

    public class Comparer : System.Collections.IComparer
    {
        private readonly double epsilon;

        public Comparer(double epsilon)
        {
            this.epsilon = epsilon;
        }

        public int Compare(object? x, object? y)
        {
            if (x is Vb6Value xVal && y is Vb6Value yVal)
            {
                if (xVal.Value is double aD && yVal.Value is double bD)
                    return Math.Abs(aD - bD) < epsilon ? 0 : aD.CompareTo(bD);
                if (xVal.Value is float aF && yVal.Value is float bF)
                    return Math.Abs(aF - bF) < epsilon ? 0 : aF.CompareTo(bF);
                if (xVal.Value is IComparable comparable)
                    return comparable.CompareTo(yVal.Value);
                if (x.Equals(y))
                    return 0;
                return -1;
            }

            return -1;
        }
    }

    protected string ConvertToVb6Value(object? value)
    {
        return value switch
        {
            null => "Null",
            bool b => b ? "True" : "False",
            int i => i.ToString(),
            float f => f.ToString("F") + "!",   // VB6 float suffix
            double d => d.ToString("F") + "#",  // VB6 double suffix
            string s => $"\"{s}\"",
            _ => throw new ArgumentException("Unsupported type")
        };
    }

    protected void AssertDebugLog(List<Vb6Value> expected)
    {
        CollectionAssert.AreEqual(expected, debug, new Comparer(0.001));
    }

    protected async Task Run(string code)
    {
        var vb = new BasicInterpreter(new MockStdLib(), context, rootEnv, code);
        await vb.Execute();
    }

    private class DebugProxy(List<Vb6Value> list) : ICSharpProxy
    {
        private readonly List<Vb6Value> list = list;

        public void Call(string method, List<Vb6Value> args)
        {
            if (method == "Print")
            {
                list.Add(args[0]);
                Console.WriteLine(args[0]);
            }
            else
                throw new Exception("No method named " + method);
        }
    }

    private class MockStdLib : IBasicStandardLibrary
    {
        public async Task<MessageBoxResult> MsgBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) => default;
        public async Task<string?> InputBox(string prompt, string title, string defaultText) => default;
    }
}