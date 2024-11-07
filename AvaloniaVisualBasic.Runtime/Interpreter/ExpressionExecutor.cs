using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Avalonia.Controls;
using AvaloniaVisualBasic.Runtime.AvaloniaInterop;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public partial class ExpressionExecutor : VB6Visitor<Task<object?>>
{
    private readonly BasicInterpreter interpreter;
    private readonly ExecutionEnvironment env;

    protected override Task<object?> DefaultResult { get; } = Task.FromResult<object?>(null);

    public ExpressionExecutor(BasicInterpreter interpreter,
        ExecutionEnvironment env)
    {
        this.interpreter = interpreter;
        this.env = env;
    }

    public async Task<Vb6Value?> EvaluateFunction(string name, List<Vb6Value> args)
    {
        return await interpreter.BuiltIns.EvaluateBuiltInFunction(name, args);
    }

    public async Task<(Vb6Value, Vb6Value)> GetTwoValues(VB6Parser.ValueStmtContext[] context)
    {
        if (context.Length != 2)
            throw new Exception("This should only be called for two operands instructions");
        var left = await EvaluateValue(context[0]);
        var right = await EvaluateValue(context[1]);
        return (left, right);
    }

    public async Task<(Vb6Value, Vb6Value)> GetTwoValuesSameTypesOrNull(VB6Parser.ValueStmtContext[] context)
    {
        var (leftValue, rightValue) = await GetTwoValues(context);
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return (leftValue, rightValue);

        if (leftValue.Type == Vb6Value.ValueType.Double)
        {
            if (rightValue.Type == Vb6Value.ValueType.Integer)
                rightValue = new Vb6Value((double)(int)rightValue.Value!);
            else if (rightValue.Type == Vb6Value.ValueType.Single)
                rightValue = new Vb6Value((double)(float)rightValue.Value!);
            else if (rightValue.Type == Vb6Value.ValueType.EmptyVariant)
                rightValue = new Vb6Value(0.0);
        }
        else if (leftValue.Type == Vb6Value.ValueType.Single)
        {
            if (rightValue.Type == Vb6Value.ValueType.Integer)
                rightValue = new Vb6Value((float)(int)rightValue.Value!);
            else if (rightValue.Type == Vb6Value.ValueType.Double)
                leftValue = new Vb6Value((double)(float)leftValue.Value!);
            else if (rightValue.Type == Vb6Value.ValueType.EmptyVariant)
                rightValue = new Vb6Value(0.0f);
        }
        else if (leftValue.Type == Vb6Value.ValueType.Integer)
        {
            if (rightValue.Type == Vb6Value.ValueType.Single)
                leftValue = new Vb6Value((float)(int)leftValue.Value!);
            else if (rightValue.Type == Vb6Value.ValueType.Double)
                leftValue = new Vb6Value((double)(int)leftValue.Value!);
            else if (rightValue.Type == Vb6Value.ValueType.EmptyVariant)
                rightValue = new Vb6Value(0);
        }
        else if (leftValue.Type == Vb6Value.ValueType.EmptyVariant)
        {
            if (rightValue.Type == Vb6Value.ValueType.Integer)
                leftValue = new Vb6Value(0);
            else if (rightValue.Type == Vb6Value.ValueType.Single)
                leftValue = new Vb6Value(0.0f);
            else if (rightValue.Type == Vb6Value.ValueType.Double)
                leftValue = new Vb6Value(0.0);
        }

        if (leftValue.Type != rightValue.Type)
            throw new VBRunTimeException(context[0], VBStandardError.TypeMismatch);
        return (leftValue, rightValue);
    }

    public async Task<(Vb6Value, Vb6Value)> GetTwoValuesSameTypes(VB6Parser.ValueStmtContext[] context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypesOrNull(context);
        if (leftValue.Type != rightValue.Type)
            throw new VBRunTimeException(context[0], VBStandardError.TypeMismatch);
        return (leftValue, rightValue);
    }


    public override async Task<object?> VisitChildren(IRuleNode node)
    {
      object? result = null;
      int childCount = node.ChildCount;
      for (int i = 0; i < childCount; ++i) // && this.ShouldVisitNextChild(node, result)
      {
          object? nextResult = await node.GetChild(i).Accept<Task<object?>>((IParseTreeVisitor<Task<object?>>) this);
          result = nextResult; //this.AggregateResult(result, Task.FromResult(nextResult));
      }
      return result;
    }

    public async Task<Vb6Value> EvaluateValue(IParseTree arg)
    {
        if (await Visit(arg) is not Vb6Value vb6Value)
            throw new NotImplementedException($"{arg.GetType()} expression is not supported");
        return vb6Value;
    }

    public async Task<List<Vb6Value>> EvaluateCallArgs(VB6Parser.ArgsCallContext? context)
    {
        List<Vb6Value> callArgs = new();
        if (context != null)
        {
            var args = context.argCall();
            foreach (var arg in args)
            {
                if (arg.BYREF() != null)
                    throw new NotImplementedException("ByReference arguments are not supported");
                if (arg.PARAMARRAY() != null)
                    throw new NotImplementedException("PARAMARRAY arguments are not supported");

                callArgs.Add(await EvaluateValue(arg.valueStmt()));
            }
        }

        return callArgs;
    }

    public async Task<string> ExtractIdentifier(VB6Parser.ICS_S_VariableOrProcedureCallContext context)
    {
        if (context.typeHint() != null)
            throw new NotImplementedException("Type hint is not supported");
        if (context.dictionaryCallStmt() != null)
            throw new NotImplementedException("dictionaryCallStmt is not supported");
        var identifier = context.ambiguousIdentifier().GetText();
        return identifier;
    }


    // EXPRESSION
    public override async Task<object?> VisitVsLiteral(VB6Parser.VsLiteralContext literalContext)
    {
        if (literalContext.literal().STRINGLITERAL() is { } stringliteral)
        {
            var str = stringliteral.GetText().Substring(1, stringliteral.GetText().Length - 2);
            Vb6Value val = new Vb6Value(str);
            return val;
        }
        if (literalContext.literal().INTEGERLITERAL() is { } integerliteral)
        {
            Vb6Value val = new Vb6Value(int.Parse(integerliteral.GetText()));
            return val;
        }
        if (literalContext.literal().DOUBLELITERAL() is { } doubleliteral)
        {
            var text = doubleliteral.GetText();
            if (text.EndsWith("#"))
                return new Vb6Value(double.Parse(text.Substring(0, text.Length - 1)));
            if (text.EndsWith("!"))
                return new Vb6Value(float.Parse(text.Substring(0, text.Length - 1)));
            return new Vb6Value(float.Parse(text));
        }
        if (literalContext.literal().TRUE() is { })
        {
            Vb6Value val = new Vb6Value(true);
            return val;
        }
        if (literalContext.literal().FALSE() is { })
        {
            Vb6Value val = new Vb6Value(false);
            return val;
        }
        if (literalContext.literal().NULL() is { })
        {
            return Vb6Value.Null;
        }
        if (literalContext.literal().COLORLITERAL() is { } colorliteral)
        {
            VBColor.TryParse(colorliteral.GetText(), out var color);
            return new Vb6Value(color);
        }
        else
        {
            throw new NotImplementedException($"{literalContext.literal().GetChild(0)} literal is not supported");
        }
    }

    public override async Task<object?> VisitImplicitCallStmt_InStmt(VB6Parser.ImplicitCallStmt_InStmtContext context)
    {
        var identifier = await ExtractIdentifier(context.iCS_S_VariableOrProcedureCall()) ?? throw new Exception("Null variable name");
        if (!interpreter.ExecutionContext.TryGetVariable(env, identifier, out var variable))
            throw new VBVariableNotDefinedException(identifier);
        return variable;
    }

    public override async Task<object?> VisitICS_S_VariableOrProcedureCall(VB6Parser.ICS_S_VariableOrProcedureCallContext context)
    {
        var identifier = await ExtractIdentifier(context);
        if (interpreter.ExecutionContext.TryGetVariable(env, identifier, out var var))
            return var;
        throw new VBVariableNotDefinedException(identifier);
    }

    public override async Task<object?> VisitVsICS(VB6Parser.VsICSContext icsContext)
    {
        if (icsContext.implicitCallStmt_InStmt().iCS_S_VariableOrProcedureCall() is { } varOrProcCall)
        {
            if (varOrProcCall.typeHint() != null)
                throw new NotImplementedException("Type hint is not supported");
            if (varOrProcCall.dictionaryCallStmt() != null)
                throw new NotImplementedException("dictionaryCallStmt is not supported");
            var identifier = varOrProcCall.ambiguousIdentifier().GetText();

            if (!interpreter.ExecutionContext.TryGetVariable(env, identifier, out var variable))
            {
                if (interpreter.BuiltIns.TryGetBuiltInConstant(identifier, out var builtInConst))
                    return builtInConst;
                else
                    throw new VBVariableNotDefinedException(identifier);
            }

            return variable;
        }
        else if (icsContext.implicitCallStmt_InStmt().iCS_S_MembersCall() is { } membersCall)
        {
            if (membersCall.dictionaryCallStmt() != null)
                throw new NotImplementedException($"dictionaryCall not supported");

            if (membersCall.iCS_S_VariableOrProcedureCall() is { } varOrProCall)
            {
                var variable = await EvaluateValue(varOrProCall);

                if (membersCall.iCS_S_MemberCall().Length != 1)
                    throw new NotImplementedException("Only single member call (single dot) is supported as of now");

                var memberIdentifier = membersCall.iCS_S_MemberCall()[0].GetText().TrimStart('.') ?? throw new Exception("Null member name");

                if (variable.Type is not Vb6Value.ValueType.Control ||
                    variable.Value is not Control control)
                    throw new VBMethodOrDataMemberNotFoundException(memberIdentifier, variable.Type);

                var props = VBProperties.PropertiesByName.GetValueOrDefault(memberIdentifier, []);

                foreach (var prop in props)
                {
                    if (AvaloniaInteroperability.TryGet(control, prop, out var value))
                        return value;
                }

                throw new VBMethodOrDataMemberNotFoundException(memberIdentifier, variable.Type);
            }
            else
            {
                throw new NotImplementedException($"{membersCall} is not implemented yet");
            }
        }
        else if (icsContext.implicitCallStmt_InStmt().iCS_S_ProcedureOrArrayCall() is { } procOrArrayCall)
        {
            if (procOrArrayCall.dictionaryCallStmt() != null)
                throw new NotImplementedException($"dictionaryCall not supported");

            if (procOrArrayCall.ambiguousIdentifier() == null)
                throw new NotImplementedException($"only proc call supportedhere");

            if (procOrArrayCall.typeHint() != null)
                throw new NotImplementedException($"typehint not supported");

            if (procOrArrayCall.argsCall().Length != 1)
                throw new NotImplementedException($"onyl single argsCall supported");

            var name = procOrArrayCall.ambiguousIdentifier().GetText();
            var args = await EvaluateCallArgs(procOrArrayCall.argsCall(0));
            if (await EvaluateFunction(name, args) is { } builtInResult)
                return builtInResult;

            throw new VBSubOrFunctionNotDefinedException(name);
        }
        else
        {
            throw new NotImplementedException($"{icsContext} is not supported");
        }
    }

    public override async Task<object?> VisitVsAmp(VB6Parser.VsAmpContext context)
    {
        var (leftValue, rightValue) = await GetTwoValues(context.valueStmt());
        if (leftValue.IsNull && rightValue.IsNull)
            return Vb6Value.Null;
        if (leftValue.IsNull)
            return new Vb6Value(rightValue.Value!.ToString());
        if (rightValue.IsNull)
            return new Vb6Value(leftValue.Value!.ToString());
        return new Vb6Value(leftValue.Value!.ToString() + rightValue.Value!.ToString());
    }

    public override async Task<object?> VisitVsAdd(VB6Parser.VsAddContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (TryUnpack<int>(leftValue, rightValue, out var leftInt, out var rightInt))
            return new Vb6Value(leftInt + rightInt);
        if (TryUnpack<float>(leftValue, rightValue, out var leftFloat, out var rightFloat))
            return new Vb6Value(leftFloat + rightFloat);
        if (TryUnpack<double>(leftValue, rightValue, out var leftDouble, out var rightDouble))
            return new Vb6Value(leftDouble + rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsMinus(VB6Parser.VsMinusContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;

        if (TryUnpack<int>(leftValue, rightValue, out var leftInt, out var rightInt))
            return new Vb6Value(leftInt - rightInt);
        if (TryUnpack<float>(leftValue, rightValue, out var leftFloat, out var rightFloat))
            return new Vb6Value(leftFloat - rightFloat);
        if (TryUnpack<double>(leftValue, rightValue, out var leftDouble, out var rightDouble))
            return new Vb6Value(leftDouble - rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsMult(VB6Parser.VsMultContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (TryUnpack<int>(leftValue, rightValue, out var leftInt, out var rightInt))
            return new Vb6Value(leftInt * rightInt);
        if (TryUnpack<float>(leftValue, rightValue, out var leftFloat, out var rightFloat))
            return new Vb6Value(leftFloat * rightFloat);
        if (TryUnpack<double>(leftValue, rightValue, out var leftDouble, out var rightDouble))
            return new Vb6Value(leftDouble * rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsMod(VB6Parser.VsModContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack<int>(leftValue, rightValue, out var leftInt, out var rightInt))
            return new Vb6Value(leftInt % rightInt);
        if (TryUnpack<float>(leftValue, rightValue, out var leftFloat, out var rightFloat))
            return new Vb6Value(leftFloat % rightFloat);
        if (TryUnpack<double>(leftValue, rightValue, out var leftDouble, out var rightDouble))
            return new Vb6Value(leftDouble % rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public async override Task<object?> VisitVsDiv(VB6Parser.VsDivContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;

        var div = context.DIV().GetText() == "/";
        if (TryUnpack<int>(leftValue, rightValue, out var leftInt, out var rightInt))
        {
            return div ? new Vb6Value(leftInt * 1.0 / rightInt) : new Vb6Value(leftInt / rightInt);
        }

        if (TryUnpack<float>(leftValue, rightValue, out var leftFloat, out var rightFloat))
        {
            return div ? new Vb6Value(leftFloat / rightFloat) : new Vb6Value((int)(leftFloat / rightFloat));
        }

        if (TryUnpack<double>(leftValue, rightValue, out var leftDouble, out var rightDouble))
        {
            return div ? new Vb6Value(leftDouble / rightDouble) : new Vb6Value((int)(leftDouble / rightDouble));
        }
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsPow(VB6Parser.VsPowContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (TryUnpack<int>(leftValue, rightValue, out var leftInt, out var rightInt))
            return new Vb6Value(Math.Pow(leftInt, rightInt));
        if (TryUnpack<float>(leftValue, rightValue, out var leftFloat, out var rightFloat))
            return new Vb6Value(Math.Pow(leftFloat, rightFloat));
        if (TryUnpack<double>(leftValue, rightValue, out var leftDouble, out var rightDouble))
            return new Vb6Value(Math.Pow(leftDouble, rightDouble));
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsEq(VB6Parser.VsEqContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypesOrNull(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        return new Vb6Value(leftValue.Equals(rightValue));
    }

    public override async Task<object?> VisitVsNeq(VB6Parser.VsNeqContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypesOrNull(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        return new Vb6Value(!leftValue.Equals(rightValue));
    }

    public override async Task<object?> VisitVsLt(VB6Parser.VsLtContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt < rightInt);
        if (TryUnpack(leftValue, rightValue, out float leftFloat, out float rightFloat))
            return (Vb6Value)(leftFloat < rightFloat);
        if (TryUnpack(leftValue, rightValue, out double leftDouble, out double rightDouble))
            return (Vb6Value)(leftDouble < rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsGt(VB6Parser.VsGtContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt > rightInt);
        if (TryUnpack(leftValue, rightValue, out float leftFloat, out float rightFloat))
            return (Vb6Value)(leftFloat > rightFloat);
        if (TryUnpack(leftValue, rightValue, out double leftDouble, out double rightDouble))
            return (Vb6Value)(leftDouble > rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsLeq(VB6Parser.VsLeqContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt <= rightInt);
        if (TryUnpack(leftValue, rightValue, out float leftFloat, out float rightFloat))
            return (Vb6Value)(leftFloat <= rightFloat);
        if (TryUnpack(leftValue, rightValue, out double leftDouble, out double rightDouble))
            return (Vb6Value)(leftDouble <= rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsGeq(VB6Parser.VsGeqContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt >= rightInt);
        if (TryUnpack(leftValue, rightValue, out float leftFloat, out float rightFloat))
            return (Vb6Value)(leftFloat >= rightFloat);
        if (TryUnpack(leftValue, rightValue, out double leftDouble, out double rightDouble))
            return (Vb6Value)(leftDouble >= rightDouble);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsAnd(VB6Parser.VsAndContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt & rightInt);
        if (TryUnpack(leftValue, rightValue, out bool leftBool, out bool rightBool))
            return (Vb6Value)(leftBool && rightBool);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsOr(VB6Parser.VsOrContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt | rightInt);
        if (TryUnpack(leftValue, rightValue, out bool leftBool, out bool rightBool))
            return (Vb6Value)(leftBool || rightBool);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsXor(VB6Parser.VsXorContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypes(context.valueStmt());
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(leftInt ^ rightInt);
        if (TryUnpack(leftValue, rightValue, out bool leftBool, out bool rightBool))
            return (Vb6Value)(leftBool ^ rightBool);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsNegation(VB6Parser.VsNegationContext context)
    {
        var value = await EvaluateValue(context.valueStmt());
        if (value.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (TryUnpack(value, out int leftInt))
            return (Vb6Value)(-leftInt);
        if (TryUnpack(value, out float leftFloat))
            return (Vb6Value)(-leftFloat);
        if (TryUnpack(value, out double leftDouble))
            return (Vb6Value)(-leftDouble);
        if (TryUnpack(value, out bool leftBool))
            return (Vb6Value)(!leftBool);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsNot(VB6Parser.VsNotContext context)
    {
        var value = await EvaluateValue(context.valueStmt());
        if (value.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (TryUnpack(value, out int leftInt))
            return (Vb6Value)(~leftInt);
        if (TryUnpack(value, out bool leftBool))
            return (Vb6Value)(!leftBool);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsEqv(VB6Parser.VsEqvContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypesOrNull(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null || rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (TryUnpack(leftValue, rightValue, out int leftInt, out int rightInt))
            return (Vb6Value)(Eqv(leftInt, rightInt));
        if (TryUnpack(leftValue, rightValue, out bool leftBool, out bool rightBool))
            return (Vb6Value)(leftBool == rightBool);
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<object?> VisitVsImp(VB6Parser.VsImpContext context)
    {
        var (leftValue, rightValue) = await GetTwoValuesSameTypesOrNull(context.valueStmt());
        if (leftValue.Type == Vb6Value.ValueType.Null &&
            rightValue.Type == Vb6Value.ValueType.Null)
            return Vb6Value.Null;
        if (leftValue.Type == Vb6Value.ValueType.Null && TryUnpack(rightValue, out bool rbool))
            return rbool ? (Vb6Value)true : Vb6Value.Null;
        if (rightValue.Type == Vb6Value.ValueType.Null && TryUnpack(leftValue, out bool lbool))
            return lbool ? Vb6Value.Null : (Vb6Value)true;
        if (TryUnpack(leftValue, rightValue, out bool leftBool, out bool rightBool))
            return (Vb6Value)(!leftBool || rightBool);
        if (TryUnpack(leftValue, rightValue, out int leftint, out int rightint))
            return (Vb6Value)(Imp(leftint, rightint));
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override Task<object?> VisitVsAddressOf(VB6Parser.VsAddressOfContext context) => throw new NotImplementedException("ADDRESSOF is not implemented");

    public override async Task<object?> VisitVsStruct(VB6Parser.VsStructContext context)
    {
        if (context.valueStmt().Length != 1)
            throw new NotImplementedException("Only single element supported");

        return await Visit(context.valueStmt(0));
    }

    public override Task<object?> VisitVsNew(VB6Parser.VsNewContext context) => throw new NotImplementedException("NEW is not implemented");

    public override Task<object?> VisitVsTypeOf(VB6Parser.VsTypeOfContext context) => throw new NotImplementedException("TypeOf is not implemented");

    public override Task<object?> VisitVsAssign(VB6Parser.VsAssignContext context) => throw new NotImplementedException("Assign is not implemented");

    public override Task<object?> VisitVsLike(VB6Parser.VsLikeContext context) => throw new NotImplementedException("Like is not implemented");

    public override Task<object?> VisitVsIs(VB6Parser.VsIsContext context) => throw new NotImplementedException("Is is not implemented");

    public override async Task<object?> VisitVsMid(VB6Parser.VsMidContext context)
    {
        var args = await EvaluateCallArgs(context.midStmt().argsCall());
        return await interpreter.BuiltIns.EvaluateBuiltInFunction("Mid", args);
    }

    private static int Eqv(int expression1, int expression2) => ~(expression1 ^ expression2);
    private static int Imp(int expression1, int expression2) => ~expression1 | expression2;


    public override async Task<object?> VisitBlockStmt(VB6Parser.BlockStmtContext context)
    {
        return await base.VisitBlockStmt(context);
    }
}