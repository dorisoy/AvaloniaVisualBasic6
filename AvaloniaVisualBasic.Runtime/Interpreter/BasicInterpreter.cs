using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Classic.CommonControls.Dialogs;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public interface IBasicStandardLibrary
{
    Task<MessageBoxResult> MsgBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
    Task<string?> InputBox(string prompt, string title, string defaultText);
}

public partial class BasicInterpreter
{
    public ModuleExecutionContext ExecutionContext { get; }
    public VB6BuiltIns BuiltIns { get; }
    private readonly IBasicStandardLibrary stdLib;
    private readonly ExecutionEnvironment rootEnv;
    private readonly string code;
    private PrePass prepass;

    public BasicInterpreter(IBasicStandardLibrary stdLib,
        ModuleExecutionContext executionContext,
        ExecutionEnvironment rootEnv,
        string code)
    {
        ExecutionContext = executionContext;
        BuiltIns = new VB6BuiltIns(stdLib);
        this.stdLib = stdLib;
        this.rootEnv = rootEnv;
        this.code = code;
        var inputStream = new AntlrInputStream(new StringReader(code));
        var lexer = new VB6Lexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new VB6Parser(commonTokenStream);
        var tree = parser.startRule();

        prepass = new PrePass(rootEnv, executionContext.State);
        prepass.Visit(tree);
    }

    public async Task Execute()
    {
        var statementExecutor = new StatementExecutor(this, rootEnv);
        foreach (var block in prepass.topLevelBlocks)
        {
            await statementExecutor.Execute(block);
        }
    }

    public async Task ExecuteSub(string name, List<Vb6Value>? args = null, bool ignoreMissing = false)
    {
        if (prepass.subs.TryGetValue(name, out var sub))
        {
            var body = sub.Item1.block();
            if (body == null)
                return;
            var env = sub.Item2;
            var statementExecutor = new StatementExecutor(this, env);
            await statementExecutor.Execute(body);
        }
        else if (await BuiltIns.EvaluateBuiltInFunction(name, args ?? []) is { } result)
        {

        }
        else if (!ignoreMissing)
            throw new VBCompileErrorException("Sub or Function not defined (" + name + ')');
    }
}