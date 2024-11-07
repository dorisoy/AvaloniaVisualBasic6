using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.Tests;

public class DoLoopTests : BaseVBTestFixture
{
    [Test]
    public async Task DoLoop_ShouldIterateUntilConditionIsMet()
    {
        string code = @"
            Dim i, result
            i = 1
            Do
                Debug.Print i
                i = i + 1
            Loop Until i > 3
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1), new Vb6Value(2), new Vb6Value(3)]);
    }

    [Test]
    public async Task DoLoopWhile_ShouldIterateWhileConditionIsTrue()
    {
        string code = @"
            Dim i, result
            i = 1
            Do
                Debug.Print i
                i = i + 1
            Loop While i <= 3
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1), new Vb6Value(2), new Vb6Value(3)]);
    }

    [Test]
    public async Task DoWhileLoop_ShouldIterateWhileConditionIsTrue()
    {
        string code = @"
            Dim i, result
            i = 1
            Do While i <= 3
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1), new Vb6Value(2), new Vb6Value(3) ]);
    }

    [Test]
    public async Task DoUntilLoop_ShouldIterateUntilConditionIsMet()
    {
        string code = @"
            Dim i, result
            i = 1
            Do Until i > 3
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1), new Vb6Value(2), new Vb6Value(3)]);
    }

    [Test]
    public async Task DoLoopUntil_ShouldExitLoopEarlyWithExitDo()
    {
        string code = @"
            Dim i, result
            i = 1
            Do
                If i = 2 Then Exit Do
                Debug.Print i
                i = i + 1
            Loop Until i > 3
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1)]);
    }

    [Test]
    public async Task DoLoopWhile_ShouldExitLoopEarlyWithExitDo()
    {
        string code = @"
            Dim i, result
            i = 1
            Do While i <= 3
                If i = 2 Then Exit Do
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1)]);
    }

    [Test]
    public async Task DoUntilLoop_ShouldExitLoopEarlyWithExitDo()
    {
        string code = @"
            Dim i, result
            i = 1
            Do Until i > 3
                If i = 2 Then Exit Do
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1)]);
    }

    [Test]
    public async Task DoLoop_ShouldSkipToNextIterationWithConditionCheck()
    {
        string code = @"
            Dim i, result
            i = 1
            Do
                If i = 2 Then 
                    i = i + 1
                    Continue Do
                End If
                Debug.Print i
                i = i + 1
            Loop Until i > 4
        ";

        await Run(code);

        AssertDebugLog([ new Vb6Value(1), new Vb6Value(3), new Vb6Value(4) ]);
    }

    [Test]
    public async Task DoWhileLoop_ShouldNotExecuteWhenConditionIsFalseInitially()
    {
        string code = @"
            Dim i, result
            i = 5
            Do While i <= 3
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([]);
    }

    [Test]
    public async Task DoUntilLoop_ShouldExecuteOnceEvenWhenConditionIsInitiallyTrue()
    {
        string code = @"
            Dim i, result
            i = 5
            Do
                Debug.Print i
                i = i + 1
            Loop Until i > 3
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(5)]);
    }

    [Test]
    public async Task DoWhileLoop_ShouldHandleNegativeValues()
    {
        string code = @"
            Dim i, result
            i = -2
            Do While i < 2
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(-2), new Vb6Value(-1), new Vb6Value(0), new Vb6Value(1)]);
    }

    [Test]
    public async Task DoUntilLoop_ShouldStopWhenVariableReachesThreshold()
    {
        string code = @"
            Dim i, result
            i = 0
            Do Until i = 3
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(0), new Vb6Value(1), new Vb6Value(2)]);
    }

    [Test]
    public async Task DoLoopWhile_ShouldExitWhenConditionChangesMidLoop()
    {
        string code = @"
            Dim i, result
            i = 1
            Do
                Debug.Print i
                i = i + 1
                If i > 2 Then Exit Do
            Loop While i <= 5
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(1), new Vb6Value(2)]);
    }

    [Test]
    public async Task DoUntilLoop_ShouldSkipOutputWhenExitDoIsCalledImmediately()
    {
        string code = @"
            Dim i, result
            i = 1
            Do Until i > 3
                Exit Do
                Debug.Print i
                i = i + 1
            Loop
        ";

        await Run(code);

        AssertDebugLog([]);
    }
}