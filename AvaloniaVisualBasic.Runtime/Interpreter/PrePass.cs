using System;
using System.Collections.Generic;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public class PrePass : VB6BaseVisitor<object?>
{
    private readonly ExecutionEnvironment rootEnv;
    private readonly ExecutionState state;
    public Dictionary<string, (VB6Parser.SubStmtContext, ExecutionEnvironment)> subs = new();
    public List<VB6Parser.BlockContext> topLevelBlocks = new();
    public bool RequireVariableDefinitions { get; private set; }

    public PrePass(ExecutionEnvironment rootEnv, ExecutionState state)
    {
        this.rootEnv = rootEnv;
        this.state = state;
    }

    public override object? VisitModuleBlock(VB6Parser.ModuleBlockContext context)
    {
        topLevelBlocks.Add(context.block());
        Visit(context.block());
        return default;
    }

    public override object? VisitOptionBaseStmt(VB6Parser.OptionBaseStmtContext context)
        => throw new NotImplementedException("Option Base not supported");

    public override object? VisitOptionCompareStmt(VB6Parser.OptionCompareStmtContext context)
        => throw new NotImplementedException("Option compare not supported");

    public override object? VisitOptionPrivateModuleStmt(VB6Parser.OptionPrivateModuleStmtContext context)
        => throw new NotImplementedException("Option private module not supported");

    public override object? VisitOptionExplicitStmt(VB6Parser.OptionExplicitStmtContext context)
    {
        RequireVariableDefinitions = true;
        return default;
    }

    public override object? VisitVariableStmt(VB6Parser.VariableStmtContext context)
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

                var location = state.Alloc(value);
                rootEnv.DefineVariable(subStmt.ambiguousIdentifier().GetText(), location);
            }
        }
        else
            throw new NotImplementedException("non dim variables not supported");

        return default;
    }

    public override object? VisitDeclareStmt(VB6Parser.DeclareStmtContext context)
        => throw new NotImplementedException("DECLARE not supported");

    public override object? VisitEnumerationStmt(VB6Parser.EnumerationStmtContext context)
        => throw new NotImplementedException("Enum not supported");

    public override object? VisitEventStmt(VB6Parser.EventStmtContext context)
        => throw new NotImplementedException("Event not supported");

    public override object? VisitMacroIfThenElseStmt(VB6Parser.MacroIfThenElseStmtContext context)
        => throw new NotImplementedException("macro if then else not supported");

    public override object? VisitPropertyGetStmt(VB6Parser.PropertyGetStmtContext context)
        => throw new NotImplementedException("Property not implemented");

    public override object? VisitPropertySetStmt(VB6Parser.PropertySetStmtContext context)
        => throw new NotImplementedException("Property not implemented");

    public override object? VisitPropertyLetStmt(VB6Parser.PropertyLetStmtContext context)
        => throw new NotImplementedException("Property not implemented");

    public override object? VisitTypeStmt(VB6Parser.TypeStmtContext context)
        => throw new NotImplementedException("Type not implemented");

    public override object? VisitFunctionStmt(VB6Parser.FunctionStmtContext context)
    {
        throw new NotImplementedException("TODO");
    }

    public override object? VisitSubStmt(VB6Parser.SubStmtContext context)
    {
        subs[context.ambiguousIdentifier().GetText()] = (context, rootEnv.Clone());
        return default;
    }
}