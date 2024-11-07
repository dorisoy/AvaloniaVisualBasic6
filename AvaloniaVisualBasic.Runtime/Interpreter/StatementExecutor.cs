using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Avalonia.Controls;
using AvaloniaVisualBasic.Runtime.AvaloniaInterop;
using AvaloniaVisualBasic.Runtime.Components;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public partial class StatementExecutor : VB6Visitor<Task<ControlFlow>>
{
    private readonly BasicInterpreter interpreter;
    private readonly ExecutionEnvironment currentEnv;

    private ExpressionExecutor expressionExecutor => new ExpressionExecutor(interpreter, currentEnv);

    public StatementExecutor(BasicInterpreter interpreter,
        ExecutionEnvironment currentEnv)
    {
        this.interpreter = interpreter;
        this.currentEnv = currentEnv;
    }

    public override async Task<ControlFlow> VisitBlock(VB6Parser.BlockContext context)
    {
        foreach (var stmt in context.blockStmt())
        {
            var ret = await Visit(stmt);
            if (ret != ControlFlow.Nothing)
                return ret;
        }

        return ControlFlow.Nothing;
    }

    public override async Task<ControlFlow> VisitAppActivateStmt(VB6Parser.AppActivateStmtContext context)
    {
        throw new NotImplementedException("AppActivate not implemented");
    }

    public override async Task<ControlFlow> VisitAttributeStmt(VB6Parser.AttributeStmtContext context)
    {
        throw new NotImplementedException("Attribute not implemented");
    }

    public override async Task<ControlFlow> VisitBeepStmt(VB6Parser.BeepStmtContext context)
    {
        throw new NotImplementedException("Beep not implemented");
    }

    public override async Task<ControlFlow> VisitChDirStmt(VB6Parser.ChDirStmtContext context)
    {
        var dir = await expressionExecutor.EvaluateValue(context.valueStmt());
        if (TryUnpack(dir, out string str))
            Environment.CurrentDirectory = str;
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<ControlFlow> VisitChDriveStmt(VB6Parser.ChDriveStmtContext context)
    {
        var drive = await expressionExecutor.EvaluateValue(context.valueStmt());
        if (TryUnpack(drive, out string str))
            Environment.CurrentDirectory = str;
        throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<ControlFlow> VisitCloseStmt(VB6Parser.CloseStmtContext context)
    {
        throw new NotImplementedException("Close not implemented");
    }

    public override async Task<ControlFlow> VisitConstStmt(VB6Parser.ConstStmtContext context)
    {
        throw new NotImplementedException("Const not implemented");
    }

    public override async Task<ControlFlow> VisitDateStmt(VB6Parser.DateStmtContext context)
    {
        throw new NotImplementedException("Date not implemented");
    }

    public override async Task<ControlFlow> VisitDeleteSettingStmt(VB6Parser.DeleteSettingStmtContext context)
    {
        throw new NotImplementedException("DeleteSetting not implemented");
    }

    public override async Task<ControlFlow> VisitDeftypeStmt(VB6Parser.DeftypeStmtContext context)
    {
        throw new NotImplementedException("Deftype not implemented");
    }

    public override async Task<ControlFlow> VisitDoBlockLoop(VB6Parser.DoBlockLoopContext context)
    {
        while (true)
        {
            var result = await Visit(context.block());
            if (result == ControlFlow.ExitDo)
                return ControlFlow.Nothing;
            if (result == ControlFlow.ContinueDo)
                continue;
            if (result != ControlFlow.Nothing)
                return result;
        }
    }

    public override async Task<ControlFlow> VisitDoBlockWhileLoop(VB6Parser.DoBlockWhileLoopContext context)
    {
        var until = context.UNTIL() != null;
        while (true)
        {
            var result = await Visit(context.block());
            if (result == ControlFlow.ExitDo)
                return ControlFlow.Nothing;
            if (result == ControlFlow.ContinueDo)
                continue;
            if (result != ControlFlow.Nothing)
                return result;

            var condition = await expressionExecutor.EvaluateValue(context.valueStmt());
            bool conditionMet;
            if (TryUnpack(condition, out bool b))
                conditionMet = b;
            else if (TryUnpack(condition, out int i))
                conditionMet = i != 0;
            else if (condition.IsNull)
                conditionMet = false;
            else
                throw new VBRunTimeException(context, VBStandardError.TypeMismatch);

            if (until && conditionMet)
                return ControlFlow.Nothing;

            if (!until && !conditionMet)
                return ControlFlow.Nothing;
        }
    }

    public override async Task<ControlFlow> VisitDoWhileBlockLoop(VB6Parser.DoWhileBlockLoopContext context)
    {
        var until = context.UNTIL() != null;
        while (true)
        {
            var condition = await expressionExecutor.EvaluateValue(context.valueStmt());
            bool conditionMet;
            if (TryUnpack(condition, out bool b))
                conditionMet = b;
            else if (TryUnpack(condition, out int i))
                conditionMet = i != 0;
            else if (condition.IsNull)
                conditionMet = false;
            else
                throw new VBRunTimeException(context, VBStandardError.TypeMismatch);

            if (until && conditionMet)
                return ControlFlow.Nothing;

            if (!until && !conditionMet)
                return ControlFlow.Nothing;

            var result = await Visit(context.block());
            if (result == ControlFlow.ExitDo)
                return ControlFlow.Nothing;
            if (result == ControlFlow.ContinueDo)
                continue;
            if (result != ControlFlow.Nothing)
                return result;

        }
    }

    public override async Task<ControlFlow> VisitEndStmt(VB6Parser.EndStmtContext context)
    {
        throw new NotImplementedException("End not implemented");
    }

    public override async Task<ControlFlow> VisitEraseStmt(VB6Parser.EraseStmtContext context)
    {
        throw new NotImplementedException("Erase not implemented");
    }

    public override async Task<ControlFlow> VisitErrorStmt(VB6Parser.ErrorStmtContext context)
    {
        throw new NotImplementedException("Error not implemented");
    }

    public override async Task<ControlFlow> VisitContinueStmt(VB6Parser.ContinueStmtContext context)
    {
        if (context.CONTINUE_DO() != null)
            return ControlFlow.ContinueDo;
        throw new NotImplementedException("Unexpected " + context);
    }

    public override async Task<ControlFlow> VisitExitStmt(VB6Parser.ExitStmtContext context)
    {
        if (context.EXIT_DO() != null)
            return ControlFlow.ExitDo;
        if (context.EXIT_FOR() != null)
            return ControlFlow.ExitFor;
        if (context.EXIT_FUNCTION() != null)
            return ControlFlow.ExitFunction;
        if (context.EXIT_PROPERTY() != null)
            return ControlFlow.ExitProperty;
        if (context.EXIT_SUB() != null)
            return ControlFlow.ExitSub;
        throw new NotImplementedException("Unexpected " + context);
    }

    private void ThrowIfTypeHint(VB6Parser.TypeHintContext? typeHintContext)
    {
        if (typeHintContext != null)
            throw new NotImplementedException("Type hints not supported");
    }

    public override async Task<ControlFlow> VisitECS_ProcedureCall(VB6Parser.ECS_ProcedureCallContext context)
    {
        var subName = context.ambiguousIdentifier().GetText();
        ThrowIfTypeHint(context.typeHint());
        var callArgs = await expressionExecutor.EvaluateCallArgs(context.argsCall());
        await interpreter.ExecuteSub(subName, callArgs);
        return ControlFlow.Nothing;
    }

    public override Task<ControlFlow> VisitECS_MemberProcedureCall(VB6Parser.ECS_MemberProcedureCallContext context)
    {
        throw new NotImplementedException("VisitECS_MemberProcedureCall not implemented");
    }

    public override async Task<ControlFlow> VisitExplicitCallStmt(VB6Parser.ExplicitCallStmtContext context) => await base.VisitExplicitCallStmt(context);

    public override async Task<ControlFlow> VisitFilecopyStmt(VB6Parser.FilecopyStmtContext context)
    {
        throw new NotImplementedException("Filecopy not implemented");
    }

    public override async Task<ControlFlow> VisitForEachStmt(VB6Parser.ForEachStmtContext context)
    {
        throw new NotImplementedException("ForEach not implemented");
    }

    public override async Task<ControlFlow> VisitForNextStmt(VB6Parser.ForNextStmtContext context)
    {
        var variable = context.iCS_S_VariableOrProcedureCall().GetText();
        if (context.typeHint().Length != 0)
            throw new NotImplementedException("TypeHints not implemented");
        if (context.asTypeClause() != null)
            throw new NotImplementedException("asTypeClause not implemented");
        var from = await expressionExecutor.EvaluateValue(context.valueStmt(0));
        var to = await expressionExecutor.EvaluateValue(context.valueStmt(1));
        var step = context.valueStmt(2) is { } stepStmt ?
            await expressionExecutor.EvaluateValue(stepStmt)
            : 1;

        if (!expressionExecutor.TryUnpack(from, out int fromInt) ||
            !expressionExecutor.TryUnpack(to, out int toInt) ||
            !expressionExecutor.TryUnpack(step, out int stepInt))
            throw new Exception($"from/to/step is not an integer");

        if (!interpreter.ExecutionContext.TryGetVariable(currentEnv, variable, out _))
            interpreter.ExecutionContext.AllocVariable(currentEnv, variable, 0);

        interpreter.ExecutionContext.TryUpdateVariable(currentEnv, variable, fromInt);

        if (stepInt > 0 && fromInt > toInt)
            return default;
        if (stepInt < 0 && fromInt < toInt)
            return default;

        var block = context.block();
        for (int i = fromInt; i != toInt; i += stepInt)
        {
            var ret = await Visit(block);
            if (ret == ControlFlow.ExitFor)
                return ControlFlow.Nothing;
            if (ret != ControlFlow.Nothing)
                return ret;
            interpreter.ExecutionContext.TryUpdateVariable(currentEnv, variable, i + stepInt);
        }
        var ret2 = await Visit(block);
        if (ret2 == ControlFlow.ExitFor)
            return ControlFlow.Nothing;
        if (ret2 != ControlFlow.Nothing)
            return ret2;
        interpreter.ExecutionContext.TryUpdateVariable(currentEnv, variable, toInt + stepInt);

        return default;
    }

    public override async Task<ControlFlow> VisitGetStmt(VB6Parser.GetStmtContext context)
    {
        throw new NotImplementedException("Get not implemented");
    }

    public override async Task<ControlFlow> VisitGoSubStmt(VB6Parser.GoSubStmtContext context)
    {
        throw new NotImplementedException("GoSub not implemented");
    }

    public override async Task<ControlFlow> VisitGoToStmt(VB6Parser.GoToStmtContext context)
    {
        throw new NotImplementedException("GoTo not implemented");
    }

    public override async Task<ControlFlow> VisitBlockIfThenElse(VB6Parser.BlockIfThenElseContext context)
    {
        var val = await expressionExecutor.EvaluateValue(context.ifBlockStmt().ifConditionStmt().valueStmt());
        if (val.Type != Vb6Value.ValueType.Boolean)
            throw new Exception("IF doesn't contain a bool expression");
        if (val.Value is true)
            return await Visit(context.ifBlockStmt().block());
        else
        {
            bool matched = false;
            foreach (var elseIf in context.ifElseIfBlockStmt())
            {
                val = await expressionExecutor.EvaluateValue(elseIf.ifConditionStmt().valueStmt());
                if (val.Type != Vb6Value.ValueType.Boolean)
                    throw new Exception("IF doesn't contain a bool expression");
                if (val.Value is true)
                {
                    return await Visit(elseIf.block());
                }
            }

            if (!matched)
            {
                if (context.ifElseBlockStmt() != null)
                {
                    return await Visit(context.ifElseBlockStmt().block());
                }
            }
        }

        return default;
    }

    public override async Task<ControlFlow> VisitInlineIfThenElse(VB6Parser.InlineIfThenElseContext context)
    {
        var condition = await expressionExecutor.EvaluateValue(context.ifConditionStmt());
        if (TryUnpack(condition, out bool conditionMet))
        {
            if (conditionMet)
                return await Visit(context.blockStmt(0));
            else
            {
                if (context.blockStmt(1) is { } @else)
                    return await Visit(@else);
            }

            return ControlFlow.Nothing;
        }
        else
            throw new VBRunTimeException(context, VBStandardError.TypeMismatch);
    }

    public override async Task<ControlFlow> VisitImplementsStmt(VB6Parser.ImplementsStmtContext context)
    {
        throw new NotImplementedException("Implements not implemented");
    }

    public override async Task<ControlFlow> VisitInputStmt(VB6Parser.InputStmtContext context)
    {
        throw new NotImplementedException("Input not implemented");
    }

    public override async Task<ControlFlow> VisitKillStmt(VB6Parser.KillStmtContext context)
    {
        throw new NotImplementedException("Kill not implemented");
    }

    public override async Task<ControlFlow> VisitLetStmt(VB6Parser.LetStmtContext context)
    {
        var value = await expressionExecutor.EvaluateValue(context.valueStmt());

        if (context.PLUS_EQ() != null || context.MINUS_EQ() != null)
            throw new NotImplementedException("+-/-= not implemented in variable assignment");

        if (context.implicitCallStmt_InStmt().iCS_S_VariableOrProcedureCall() is { } varOrProcCall)
        {
            var identifier = varOrProcCall.GetText() ?? throw new Exception("Null variable name");
            if (!interpreter.ExecutionContext.TryUpdateVariable(currentEnv, identifier, value))
                throw new Exception("Variable " + identifier + " is not declared");

            return ControlFlow.Nothing;
        }
        else if (context.implicitCallStmt_InStmt().iCS_S_MembersCall() is { } membersCall)
        {
            if (membersCall.dictionaryCallStmt() != null ||
                membersCall.iCS_S_ProcedureOrArrayCall() != null)
                throw new NotImplementedException("dict or procorarray not supported yet");
            var identifier = membersCall.iCS_S_VariableOrProcedureCall().GetText() ?? throw new Exception("Null variable name");
            if (!interpreter.ExecutionContext.TryGetVariable(currentEnv, identifier, out var variable))
                throw new Exception("Can't find variable " + identifier);

            if (membersCall.iCS_S_MemberCall().Length != 1)
                throw new NotImplementedException("Only single member call (single dot) is supported as of now");

            var memberIdentifier = membersCall.iCS_S_MemberCall()[0].GetText().TrimStart('.') ?? throw new Exception("Null member name");

            if (variable.Type is not Vb6Value.ValueType.Control ||
                variable.Value is not Control control)
                throw new Exception($"Variable {identifier} type {variable.Type} doesn't have member {memberIdentifier}");

            var props = VBProperties.PropertiesByName.GetValueOrDefault(memberIdentifier, []);

            foreach (var prop in props)
            {
                if (AvaloniaInteroperability.TrySet(control, prop, value))
                    return default;
            }

            throw new Exception($"Variable {identifier} type {variable.Type} doesn't have member {memberIdentifier}");
        }
        else
        {
            throw new NotImplementedException($"{context.implicitCallStmt_InStmt()} is not supported");
        }

        return default;
    }

    public override async Task<ControlFlow> VisitLineInputStmt(VB6Parser.LineInputStmtContext context)
    {
        throw new NotImplementedException("LineInput not implemented");
    }

    public override async Task<ControlFlow> VisitLineLabel(VB6Parser.LineLabelContext context)
    {
        throw new NotImplementedException("LineLabel not implemented");
    }

    public override async Task<ControlFlow> VisitLoadStmt(VB6Parser.LoadStmtContext context)
    {
        throw new NotImplementedException("Load not implemented");
    }

    public override async Task<ControlFlow> VisitLockStmt(VB6Parser.LockStmtContext context)
    {
        throw new NotImplementedException("Lock not implemented");
    }

    public override async Task<ControlFlow> VisitLsetStmt(VB6Parser.LsetStmtContext context)
    {
        throw new NotImplementedException("Lset not implemented");
    }

    public override async Task<ControlFlow> VisitMacroIfThenElseStmt(VB6Parser.MacroIfThenElseStmtContext context)
    {
        throw new NotImplementedException("MacroIfThenElse not implemented");
    }

    public override async Task<ControlFlow> VisitMidStmt(VB6Parser.MidStmtContext context)
    {
        throw new NotImplementedException("Mid not implemented");
    }

    public override async Task<ControlFlow> VisitMkdirStmt(VB6Parser.MkdirStmtContext context)
    {
        throw new NotImplementedException("Mkdir not implemented");
    }

    public override async Task<ControlFlow> VisitNameStmt(VB6Parser.NameStmtContext context)
    {
        throw new NotImplementedException("Name not implemented");
    }

    public override async Task<ControlFlow> VisitOnErrorStmt(VB6Parser.OnErrorStmtContext context)
    {
        throw new NotImplementedException("OnError not implemented");
    }

    public override async Task<ControlFlow> VisitOnGoToStmt(VB6Parser.OnGoToStmtContext context)
    {
        throw new NotImplementedException("OnGoTo not implemented");
    }

    public override async Task<ControlFlow> VisitOnGoSubStmt(VB6Parser.OnGoSubStmtContext context)
    {
        throw new NotImplementedException("OnGoSub not implemented");
    }

    public override async Task<ControlFlow> VisitOpenStmt(VB6Parser.OpenStmtContext context)
    {
        throw new NotImplementedException("Open not implemented");
    }

    public override async Task<ControlFlow> VisitPrintStmt(VB6Parser.PrintStmtContext context)
    {
        throw new NotImplementedException("Print not implemented");
    }

    public override async Task<ControlFlow> VisitPutStmt(VB6Parser.PutStmtContext context)
    {
        throw new NotImplementedException("Put not implemented");
    }

    public override async Task<ControlFlow> VisitRaiseEventStmt(VB6Parser.RaiseEventStmtContext context)
    {
        throw new NotImplementedException("RaiseEvent not implemented");
    }

    public override async Task<ControlFlow> VisitRandomizeStmt(VB6Parser.RandomizeStmtContext context)
    {
        throw new NotImplementedException("Randomize not implemented");
    }

    public override async Task<ControlFlow> VisitRedimStmt(VB6Parser.RedimStmtContext context)
    {
        throw new NotImplementedException("Redim not implemented");
    }

    public override async Task<ControlFlow> VisitResetStmt(VB6Parser.ResetStmtContext context)
    {
        throw new NotImplementedException("Reset not implemented");
    }

    public override async Task<ControlFlow> VisitResumeStmt(VB6Parser.ResumeStmtContext context)
    {
        throw new NotImplementedException("Resume not implemented");
    }

    public override async Task<ControlFlow> VisitReturnStmt(VB6Parser.ReturnStmtContext context)
    {
        throw new NotImplementedException("Return not implemented");
    }

    public override async Task<ControlFlow> VisitRmdirStmt(VB6Parser.RmdirStmtContext context)
    {
        throw new NotImplementedException("Rmdir not implemented");
    }

    public override async Task<ControlFlow> VisitRsetStmt(VB6Parser.RsetStmtContext context)
    {
        throw new NotImplementedException("Rset not implemented");
    }

    public override async Task<ControlFlow> VisitSavepictureStmt(VB6Parser.SavepictureStmtContext context)
    {
        throw new NotImplementedException("Savepicture not implemented");
    }

    public override async Task<ControlFlow> VisitSaveSettingStmt(VB6Parser.SaveSettingStmtContext context)
    {
        throw new NotImplementedException("SaveSetting not implemented");
    }

    public override async Task<ControlFlow> VisitSeekStmt(VB6Parser.SeekStmtContext context)
    {
        throw new NotImplementedException("Seek not implemented");
    }

    public override async Task<ControlFlow> VisitSelectCaseStmt(VB6Parser.SelectCaseStmtContext context)
    {
        var value = await expressionExecutor.EvaluateValue(context.valueStmt());
        foreach (var @case in context.sC_Case())
        {
            var cond = @case.sC_Cond();
            if (cond is VB6Parser.CaseCondElseContext)
            {
                return await Visit(@case.block());
            }
            else if (cond is VB6Parser.CaseCondExprContext condExpr)
            {
                foreach (var subCond in condExpr.sC_CondExpr())
                {
                    if (subCond is VB6Parser.CaseCondExprIsContext caseIs)
                    {
                        var val = await expressionExecutor.EvaluateValue(caseIs.valueStmt());
                        if (caseIs.comparisonOperator().LT() != null)
                        {
                            if (value.TryCompareTo(val) is < 0)
                                return await Visit(@case.block());
                        }
                        else if (caseIs.comparisonOperator().LEQ() != null)
                        {
                            if (value.TryCompareTo(val) is <= 0)
                                return await Visit(@case.block());
                        }
                        else if (caseIs.comparisonOperator().GT() != null)
                        {
                            if (value.TryCompareTo(val) is > 0)
                                return await Visit(@case.block());
                        }
                        else if (caseIs.comparisonOperator().GEQ() != null)
                        {
                            if (value.TryCompareTo(val) is >= 0)
                                return await Visit(@case.block());
                        }
                        else if (caseIs.comparisonOperator().EQ() != null)
                        {
                            if (value.Equals(val))
                                return await Visit(@case.block());
                        }
                        else if (caseIs.comparisonOperator().NEQ() != null)
                        {
                            if (!value.Equals(val))
                                return await Visit(@case.block());
                        }
                        else if (caseIs.comparisonOperator().IS() != null)
                        {
                            throw new NotImplementedException("Operator " + caseIs.comparisonOperator().GetText() + " not impleemented");
                        }
                        else if (caseIs.comparisonOperator().LIKE() != null)
                        {
                            throw new NotImplementedException("Operator " + caseIs.comparisonOperator().GetText() + " not impleemented");
                        }
                        else
                            throw new NotImplementedException("Operator " + caseIs.comparisonOperator().GetText() + " not impleemented");
                    }
                    else if (subCond is VB6Parser.CaseCondExprValueContext caseCondExpr)
                    {
                        var val = await expressionExecutor.EvaluateValue(caseCondExpr.valueStmt());
                        if (val.Equals(value))
                            return await Visit(@case.block());
                    }
                    else if (subCond is VB6Parser.CaseCondExprToContext caseTo)
                    {
                        var from = await expressionExecutor.EvaluateValue(caseTo.valueStmt(0));
                        var to = await expressionExecutor.EvaluateValue(caseTo.valueStmt(1));
                        if (value.TryCompareTo(from) is >= 0 && value.TryCompareTo(to) is <= 0)
                        {
                            return await Visit(@case.block());
                        }
                    }
                    else
                    {
                        throw new NotImplementedException($"Unepexected Select-Case");
                    }
                }
            }
            else
                throw new NotImplementedException($"Unepexected Select-Case");
        }

        return ControlFlow.Nothing;
    }

    public override async Task<ControlFlow> VisitSendkeysStmt(VB6Parser.SendkeysStmtContext context)
    {
        throw new NotImplementedException("Sendkeys not implemented");
    }

    public override async Task<ControlFlow> VisitSetattrStmt(VB6Parser.SetattrStmtContext context)
    {
        throw new NotImplementedException("Setattr not implemented");
    }

    public override async Task<ControlFlow> VisitSetStmt(VB6Parser.SetStmtContext context)
    {
        throw new NotImplementedException("Set not implemented");
    }

    public override async Task<ControlFlow> VisitStopStmt(VB6Parser.StopStmtContext context)
    {
        throw new NotImplementedException("Stop not implemented");
    }

    public override async Task<ControlFlow> VisitTimeStmt(VB6Parser.TimeStmtContext context)
    {
        throw new NotImplementedException("Time not implemented");
    }

    public override async Task<ControlFlow> VisitUnloadStmt(VB6Parser.UnloadStmtContext context)
    {
        throw new NotImplementedException("Unload not implemented");
    }

    public override async Task<ControlFlow> VisitUnlockStmt(VB6Parser.UnlockStmtContext context)
    {
        throw new NotImplementedException("Unlock not implemented");
    }

    public override async Task<ControlFlow> VisitVariableStmt(VB6Parser.VariableStmtContext context)
    {
        if (context.WITHEVENTS() != null)
            throw new NotImplementedException("WITHEVENTS not implemented");

        if (context.DIM() != null)
        {
            foreach (var subStmt in context.variableListStmt().variableSubStmt())
            {
                if (subStmt.typeHint() != null)
                    throw new NotImplementedException("DIM type hints not implemented");
                if (subStmt.subscripts() != null)
                    throw new NotImplementedException("DIM subscripts not implemented");
                Vb6Value value = Vb6Value.Variant;
                if (subStmt.asTypeClause() != null)
                {
                    if (subStmt.asTypeClause().NEW() != null)
                        throw new NotImplementedException("New as type not implemented");
                    if (subStmt.asTypeClause().fieldLength() != null)
                        throw new NotImplementedException("fieldLength as type not implemented");
                    if (subStmt.asTypeClause().type().complexType() != null)
                        throw new NotImplementedException("complex type as type not implemented");
                    if (subStmt.asTypeClause().type().baseType().STRING() != null)
                        value = new Vb6Value("");
                    else if (subStmt.asTypeClause().type().baseType().INTEGER() != null)
                        value = new Vb6Value(0);
                    else if (subStmt.asTypeClause().type().baseType().SINGLE() != null)
                        value = new Vb6Value(0.0f);
                    else if (subStmt.asTypeClause().type().baseType().DOUBLE() != null)
                        value = new Vb6Value(0.0);
                    else if (subStmt.asTypeClause().type().baseType().BOOLEAN() != null)
                        value = new Vb6Value(false);
                    else
                        throw new NotImplementedException("base type " + subStmt.asTypeClause().type().baseType().GetChild(0) + " not implemented");
                }
                interpreter.ExecutionContext.AllocVariable(currentEnv, subStmt.ambiguousIdentifier().GetText(), value);
            }
        }
        else
            throw new NotImplementedException("non dim variables not supported");

        return default;
    }

    public override async Task<ControlFlow> VisitWhileWendStmt(VB6Parser.WhileWendStmtContext context)
    {
        throw new NotImplementedException("WhileWend not implemented");
    }

    public override async Task<ControlFlow> VisitWidthStmt(VB6Parser.WidthStmtContext context)
    {
        throw new NotImplementedException("Width not implemented");
    }

    public override async Task<ControlFlow> VisitWithStmt(VB6Parser.WithStmtContext context)
    {
        throw new NotImplementedException("With not implemented");
    }

    public override async Task<ControlFlow> VisitWriteStmt(VB6Parser.WriteStmtContext context)
    {
        throw new NotImplementedException("Write not implemented");
    }

    public override async Task<ControlFlow> VisitICS_B_MemberProcedureCall(VB6Parser.ICS_B_MemberProcedureCallContext context)
    {
        var value = await expressionExecutor.EvaluateValue(context.implicitCallStmt_InStmt());
        var identifier = context.ambiguousIdentifier().GetText() ?? throw new Exception("Empty method identifier");
        var callArgs = await expressionExecutor.EvaluateCallArgs(context.argsCall());

        if (value.Type == Vb6Value.ValueType.CSharpProxyObject)
        {
            ((ICSharpProxy)value.Value!).Call(identifier, callArgs);
        }
        else
            throw new NotImplementedException($"Unknown method {identifier} on {value}");

        return default;
    }

    public override async Task<ControlFlow> VisitICS_B_ProcedureCall(VB6Parser.ICS_B_ProcedureCallContext context)
    {
        var subName = context.certainIdentifier().GetText();
        List<Vb6Value> callArgs = await expressionExecutor.EvaluateCallArgs(context.argsCall());
        await expressionExecutor.EvaluateFunction(subName, callArgs);
        return ControlFlow.Nothing;
    }

    public override async Task<ControlFlow> VisitImplicitCallStmt_InBlock(VB6Parser.ImplicitCallStmt_InBlockContext context) => await base.VisitImplicitCallStmt_InBlock(context);

    public async Task Execute(VB6Parser.BlockContext block)
    {
        await Visit(block);
    }

    public override async Task<ControlFlow> VisitChildren(IRuleNode node)
    {
        ControlFlow result = default;
        int childCount = node.ChildCount;
        for (int i = 0; i < childCount; ++i) // && this.ShouldVisitNextChild(node, result)
        {
            ControlFlow nextResult = await node.GetChild(i).Accept<Task<ControlFlow>>((IParseTreeVisitor<Task<ControlFlow>>) this);
            result = nextResult; //this.AggregateResult(result, Task.FromResult(nextResult));
        }
        return result;
    }
}