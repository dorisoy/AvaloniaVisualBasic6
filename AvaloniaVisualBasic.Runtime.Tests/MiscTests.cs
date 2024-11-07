using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.Tests;

public class MiscTests : BaseVBTestFixture
{
    [Test]
    public async Task DebugPrint_WithStringLiteral_ShouldOutputCorrectString()
    {
        await Run(@"Debug.Print ""Hello""");
        AssertDebugLog(["Hello"]);
    }

    [Test]
    public async Task VariableAssignment_ShouldOutputVariableValue()
    {
        await Run(@"
        Dim x
        Let x = 42
        Debug.Print x
    ");
        AssertDebugLog([new Vb6Value(42)]);
    }

    [Test]
    public async Task IfStatement_ShouldOutputBasedOnCondition()
    {
        await Run(@"
        Dim x
        Let x = 10
        If x = 10 Then
            Debug.Print ""Ten""
        Else
            Debug.Print ""Not Ten""
        End If
    ");
        AssertDebugLog(["Ten"]);
    }

    [Test]
    public async Task IfStatement_ShouldOutputBasedOnConditionNotEqual()
    {
        await Run(@"
        Dim x
        Let x = 10
        If x <> 10 Then
            Debug.Print ""Not Ten""
        Else
            Debug.Print ""Ten""
        End If
    ");
        AssertDebugLog(["Ten"]);
    }
}