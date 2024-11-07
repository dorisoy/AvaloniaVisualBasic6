using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.Tests;

public class OperatorTests : BaseVBTestFixture
{
    [TestCase(true, true, true)]
    [TestCase(true, false, false)]
    [TestCase(true, null, null)]
    [TestCase(false, true, true)]
    [TestCase(false, false, true)]
    [TestCase(false, null, true)]
    [TestCase(null, true, true)]
    [TestCase(null, false, null)]
    [TestCase(null, null, null)]
    public async Task ImpOperator_ShouldReturnExpectedResult(bool? expression1, bool? expression2, bool? expectedResult)
    {
        string code = $@"
            Dim result
            result = {ConvertToVb6Value(expression1)} IMP {ConvertToVb6Value(expression2)}
            Debug.Print result
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(expectedResult)]);
    }

    [Test]
    public async Task ImpOperator_Int_ShouldReturnExpectedResult()
    {
        await Run("Debug.Print 3 Imp 5");
        AssertDebugLog([-3]);
    }

    [TestCase(true, true, true)]
    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(true, null, null)]
    [TestCase(false, null, null)]
    [TestCase(null, true, null)]
    [TestCase(null, false, null)]
    [TestCase(null, null, null)]
    public async Task EqvOperator_ShouldReturnExpectedResult(bool? expression1, bool? expression2, bool? expectedResult)
    {
        string code = $@"
            Dim result
            result = {ConvertToVb6Value(expression1)} EQV {ConvertToVb6Value(expression2)}
            Debug.Print result
        ";

        await Run(code);

        AssertDebugLog([new Vb6Value(expectedResult)]);
    }

    [Test]
    public async Task EqvOperator_Int_ShouldReturnExpectedResult()
    {
        await Run("Debug.Print 3 Eqv 5");
        AssertDebugLog([-7]);
    }

    [TestCase(true, false)]           // Not True
    [TestCase(false, true)]           // Not False
    [TestCase(null, null)]            // Not Null
    [TestCase(0, -1)]                 // Not 0 (bitwise NOT of 0)
    [TestCase(1, -2)]                 // Not 1 (bitwise NOT of 1)
    [TestCase(255, -256)]             // Not 255 (bitwise NOT of 255)
    public async Task NotOperator_ShouldReturnExpectedResult(object? operand, object? expectedResult)
    {
        string vbOperand = ConvertToVb6Value(operand);
        string code = $@"
            Dim result
            result = Not {vbOperand}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is bool b ? new Vb6Value(b) : expectedResult is null ? Vb6Value.Null : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [TestCase("<", true, false, false, true, false, false)]
    [TestCase(">", false, false, true, false, false, true)]
    [TestCase("<=", true, true, false, true, true, false)]
    [TestCase(">=", false, true, true, false, true, true)]
    [TestCase("=", false, true, false, false, true, false)]
    [TestCase("<>", true, false, true, true, false, true)]
    public async Task ComparisonOperator_ShouldReturnExpectedResults(
        string op,
        bool intLowerResult, bool intEqualResult, bool intGreaterResult,
        bool doubleLowerResult, bool doubleEqualResult, bool doubleGreaterResult)
    {
        string code = $@"
            Dim result1, result2, result3, result4, result5, result6
            result1 = 1 {op} 2            ' int lower
            result2 = 2 {op} 2            ' int equal
            result3 = 3 {op} 2            ' int greater
            result4 = 1.5 {op} 2.5        ' double lower
            result5 = 2.5 {op} 2.5        ' double equal
            result6 = 3.5 {op} 2.5        ' double greater
            Debug.Print result1
            Debug.Print result2
            Debug.Print result3
            Debug.Print result4
            Debug.Print result5
            Debug.Print result6
        ";

        await Run(code);

        AssertDebugLog([
            new Vb6Value(intLowerResult),
            new Vb6Value(intEqualResult),
            new Vb6Value(intGreaterResult),
            new Vb6Value(doubleLowerResult),
            new Vb6Value(doubleEqualResult),
            new Vb6Value(doubleGreaterResult)
        ]);
    }

    [TestCase("=", true, false, true, false, null, null)]
    [TestCase("<>", false, true, false, true, null, null)]
    public async Task EqualityOperator_ShouldReturnExpectedResults(
        string op,
        bool boolEqualResult, bool boolNotEqualResult,
        bool stringEqualResult, bool stringNotEqualResult,
        bool? nullEqualResult, bool? nullNotEqualResult)
    {
        string code = $@"
            Dim result1, result2, result3, result4, result5, result6
            result1 = True {op} True           ' bool equal
            result2 = True {op} False          ' bool not equal
            result3 = ""hello"" {op} ""hello""     ' string equal
            result4 = ""hello"" {op} ""world""     ' string not equal
            result5 = Null {op} Null           ' null equal
            result6 = Null {op} ""text""         ' null not equal
            Debug.Print result1
            Debug.Print result2
            Debug.Print result3
            Debug.Print result4
            Debug.Print result5
            Debug.Print result6
        ";

        await Run(code);

        AssertDebugLog([
            boolEqualResult,
            boolNotEqualResult,
            stringEqualResult,
            stringNotEqualResult,
            nullEqualResult,
            nullNotEqualResult
        ]);
    }

    [TestCase(1, 2, 3)]             // int + int
    [TestCase(1, 2.5F, 3.5F)]       // int + float
    [TestCase(1, 2.5D, 3.5D)]       // int + double
    [TestCase(2.5F, 1, 3.5F)]       // float + int
    [TestCase(2.5F, 2.5F, 5.0F)]    // float + float
    [TestCase(2.5F, 2.5D, 5.0D)]    // float + double
    [TestCase(2.5D, 1, 3.5D)]       // double + int
    [TestCase(2.5D, 2.5F, 5.0D)]    // double + float
    [TestCase(2.5D, 2.5D, 5.0D)]    // double + double
    public async Task AdditionOperator_ShouldReturnExpectedResult(object operand1, object operand2, object expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} + {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is float f ? new Vb6Value(f) : expectedResult is double d ? new Vb6Value(d) : throw new Exception();

        AssertDebugLog([expectedValue]);
    }


    [TestCase(5, 2, 3)]               // int - int
    [TestCase(5, 2.5F, 2.5F)]         // int - float
    [TestCase(5, 2.5D, 2.5D)]         // int - double
    [TestCase(5.5F, 2, 3.5F)]         // float - int
    [TestCase(5.5F, 2.5F, 3.0F)]      // float - float
    [TestCase(5.5F, 2.5D, 3.0D)]      // float - double
    [TestCase(5.5D, 2, 3.5D)]         // double - int
    [TestCase(5.5D, 2.5F, 3.0D)]      // double - float
    [TestCase(5.5D, 2.5D, 3.0D)]      // double - double
    public async Task SubtractionOperator_ShouldReturnExpectedResult(object operand1, object operand2, object expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} - {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is float f ? new Vb6Value(f) : expectedResult is double d ? new Vb6Value(d) : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [TestCase(5, 2, 2.5)]             // int / int
    [TestCase(5, 2.5F, 2.0F)]          // int / float
    [TestCase(5, 2.5D, 2.0)]          // int / double
    [TestCase(5.5F, 2, 2.75F)]        // float / int
    [TestCase(5.5F, 2.5F, 2.2F)]      // float / float
    [TestCase(5.5F, 2.5D, 2.2D)]      // float / double
    [TestCase(5.5D, 2, 2.75D)]        // double / int
    [TestCase(5.5D, 2.5F, 2.2D)]      // double / float
    [TestCase(5.5D, 2.5D, 2.2D)]      // double / double
    public async Task DivisionOperator_ShouldReturnExpectedResult(object operand1, object operand2, object expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} / {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is float f ? new Vb6Value(f) : expectedResult is double d ? new Vb6Value(d) : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [TestCase(3, 2, 6)]               // int * int
    [TestCase(3, 2.5F, 7.5F)]         // int * float
    [TestCase(3, 2.5D, 7.5D)]         // int * double
    [TestCase(3.5F, 2, 7.0F)]         // float * int
    [TestCase(3.5F, 2.5F, 8.75F)]      // float * float
    [TestCase(3.5F, 2.5D, 8.75D)]      // float * double
    [TestCase(3.5D, 2, 7.0D)]         // double * int
    [TestCase(3.5D, 2.5F, 8.75D)]      // double * float
    [TestCase(3.5D, 2.5D, 8.75D)]      // double * double
    public async Task MultiplicationOperator_ShouldReturnExpectedResult(object operand1, object operand2, object expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} * {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is float f ? new Vb6Value(f) : expectedResult is double d ? new Vb6Value(d) : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [TestCase(5, 2, 1)]               // int Mod int
    // These two cases fail now, return float/double instead. TODO  [TestCase(5, 2.5F, 0)]            // int Mod float
    // These two cases fail now, return float/double instead. TODO  [TestCase(5, 2.5D, 0)]            // int Mod double
    [TestCase(5.5F, 2, 1.5F)]         // float Mod int
    [TestCase(5.5F, 2.5F, 0.5F)]      // float Mod float
    [TestCase(5.5F, 2.5D, 0.5D)]      // float Mod double
    [TestCase(5.5D, 2, 1.5D)]         // double Mod int
    [TestCase(5.5D, 2.5F, 0.5D)]      // double Mod float
    [TestCase(5.5D, 2.5D, 0.5D)]      // double Mod double
    public async Task ModulusOperator_ShouldReturnExpectedResult(object operand1, object operand2, object expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} Mod {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is float f ? new Vb6Value(f) : expectedResult is double d ? new Vb6Value(d) : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [TestCase(2, 3, 8d)]               // int ^ int
    [TestCase(2, 3.5F, 11.3137D)]     // int ^ float
    [TestCase(2, 3.5D, 11.3137D)]     // int ^ double
    [TestCase(3.5F, 2, 12.25D)]       // float ^ int
    [TestCase(3.5F, 2.5F, 22.9169D)]   // float ^ float
    [TestCase(3.5F, 2.5D, 22.9169D)]   // float ^ double
    [TestCase(3.5D, 2, 12.25D)]       // double ^ int
    [TestCase(3.5D, 2.5F, 22.9169D)]   // double ^ float
    [TestCase(3.5D, 2.5D, 22.9169D)]   // double ^ double
    public async Task PowerOperator_ShouldReturnExpectedResult(object operand1, object operand2, object expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} ^ {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is int i ? new Vb6Value(i) : expectedResult is float f ? new Vb6Value(f) : expectedResult is double d ? new Vb6Value(d) : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [TestCase("Hello", " World", "Hello World")] // String + String
    [TestCase("Value: ", 42, "Value: 42")]        // String + Int
    [TestCase("Value: ", 3.14F, "Value: 3.14")]   // String + Float
    [TestCase("Value: ", 3.14D, "Value: 3.14")]   // String + Double
    [TestCase(42, " is the answer", "42 is the answer")] // Int + String
    [TestCase(3.14F, " is pi", "3.14 is pi")]     // Float + String
    [TestCase(3.14D, " is pi", "3.14 is pi")]     // Double + String
    [TestCase(null, "Hello", "Hello")]             // Null + String
    [TestCase("Hello", null, "Hello")]             // String + Null
    [TestCase(null, null, null)]                    // Null + Null
    [TestCase("", "Hello", "Hello")]                // Empty + String
    [TestCase("Hello", "", "Hello")]                // String + Empty
    [TestCase("", "", "")]                          // Empty + Empty
    public async Task AmpersandOperator_ShouldReturnExpectedResult(object? operand1, object? operand2, object? expectedResult)
    {
        string vbOperand1 = ConvertToVb6Value(operand1);
        string vbOperand2 = ConvertToVb6Value(operand2);
        string code = $@"
            Dim result
            result = {vbOperand1} & {vbOperand2}
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is string str ? new Vb6Value(str) : expectedResult is null ? Vb6Value.Null : throw new Exception();

        AssertDebugLog([expectedValue]);
    }
}