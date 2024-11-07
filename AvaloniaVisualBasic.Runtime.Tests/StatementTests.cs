using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.Tests;

public class StatementTests : BaseVBTestFixture
{
    [TestCase(1, 5, new[] { 1, 2, 3, 4, 5, 6 })]             // Simple increment loop
    [TestCase(5, 1, new[] { 5, 4, 3, 2, 1, 0 }, -1)]             // Simple decrement loop
    [TestCase(5, 1, new[] { 5 })]             // Simple decrement loop
    [TestCase(1, 5, new[] { 1, 3, 5, 7 }, 2)]                // Increment by step 2
    [TestCase(10, 2, new[] { 10, 8, 6, 4, 2, 0 }, -2)]       // Decrement by step -2
    [TestCase(1, 1, new[] { 1, 2 })]                         // Single iteration (start = end)
    public async Task ForLoop_ShouldIterateCorrectly(int start, int end, int[] expectedValues, int step = 1)
    {
        string code = $@"
            For i = {start} To {end}" + (step == 1 ? "" : $" Step {step}") + $@"
                Debug.Print i
            Next
            Debug.Print i
        ";

        await Run(code);

        var expectedLog = expectedValues.Select(value => new Vb6Value(value)).ToList();

        AssertDebugLog(expectedLog);
    }

    [Test]
    public async Task ExitFor_ShouldTerminateLoopEarly()
    {
        string code = @"
            Dim result
            For i = 1 To 10
                If i = 2 Then
                    Exit For
                End If
                Debug.Print i
            Next
            Debug.Print i
        ";

        await Run(code);

        var expectedLog = new[]
        {
            new Vb6Value(1),
            new Vb6Value(2)
        };

        AssertDebugLog(expectedLog.ToList());
    }

    [Test]
    public async Task ExitSub_ShouldTerminateLoopEarly()
    {
        string code = @"
            Dim result
            Public Sub Test()
                For i = 1 To 10
                    If i = 2 Then
                        Exit Sub
                    End If
                    Debug.Print i
                Next
            End Sub
            Call Test
        ";

        await Run(code);

        var expectedLog = new[]
        {
            new Vb6Value(1)
        };

        AssertDebugLog(expectedLog.ToList());
    }

    [TestCase(1, "One")] // Simple case
    [TestCase(2, "Two")] // Simple case
    [TestCase(3, "Three")] // Simple case
    [TestCase(4, "Number greater than 3")] // Case with `Else`
    [TestCase(5, "Number greater than 3")] // Case with `Else`
    [TestCase(10, "Between 10 and 20")] // Case with a range
    [TestCase(15, "14, 15, 16")]
    [TestCase(0, "Out of range")] // Case with a range
    [TestCase(100, "Number greater than 3")] // Case with a range
    [TestCase(1, "One")] // Case with condition and operator
    [TestCase(20, "Between 10 and 20")] // Case with `To` range
    public async Task SelectCaseTests(int input, string expectedOutput)
    {
        string code = $@"
        Select Case {input}
            Case 1
                Debug.Print ""One""
            Case 2
                Debug.Print ""Two""
            Case 3
                Debug.Print ""Three""
            Case 14,15,16
                Debug.Print ""14, 15, 16""
            Case 10 To 20
                Debug.Print ""Between 10 and 20""
            Case Is > 3
                Debug.Print ""Number greater than 3""
            Case Else
                Debug.Print ""Out of range""
        End Select
    ";

        await Run(code);
        AssertDebugLog([new Vb6Value(expectedOutput)]);
    }
}