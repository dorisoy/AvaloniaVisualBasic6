using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.Tests;

public class BuiltInFunctionsTests : BaseVBTestFixture
{
    [TestCase(false, "Hello, World!", 1, null, "Hello, World!")]        // Full string from start
    [TestCase(false, "Hello, World!", 1, 5, "Hello")]                   // First 5 characters
    [TestCase(false, "Hello, World!", 8, 5, "World")]                   // Middle of the string
    [TestCase(false, "Hello, World!", 14, null, "")]                   // Start greater than string length
    [TestCase(false, "Hello, World!", 8, 20, "World!")]                  // Start within string, length exceeds
    [TestCase(false, "Hello, World!", 1, 0, "")]                        // Length is 0
    [TestCase(true, "Hello, World!", 0, 5, "")]                        // Start is 0 (invalid)
    [TestCase(false, "Hello, World!", 15, 5, "")]                       // Start exceeds length, returns ""
    [TestCase(false, null, 1, null, null)]                               // Null string input
    [TestCase(false, "", 1, null, "")]                                   // Empty string
    [TestCase(false, "Test", 2, 2, "es")]                                // Start within bounds, length within bounds
    public async Task MidFunction_ShouldReturnExpectedResult(bool assertThrows, string? inputString, int start, int? length, string? expectedResult)
    {
        var vbString = inputString == null ? "Null" : $@"""{inputString}""";
        string code = $@"
            Dim result
            result = Mid({vbString}, {start}{(length.HasValue ? $", {length.Value}" : "")})
            Debug.Print result
        ";

        if (assertThrows)
        {
            try
            {
                await Run(code);
                Assert.Fail("Exception expected, but nothing thrown");
            }
            catch (Exception e)
            {
                Assert.Pass();
            }
        }
        else
        {
            await Run(code);

            Vb6Value expectedValue = expectedResult is string str ? new Vb6Value(str) : expectedResult is null ? Vb6Value.Null : throw new Exception();

            AssertDebugLog([expectedValue]);
        }
    }

    [TestCase("hello", "HELLO")]                // Lowercase string
    [TestCase("HELLO", "HELLO")]                // Uppercase string
    [TestCase("Hello World!", "HELLO WORLD!")]  // Mixed case
    [TestCase("1234!@#$", "1234!@#$")]          // Non-letter characters
    [TestCase("", "")]                          // Empty string
    [TestCase(null, null)]                      // Null string
    public async Task UCaseFunction_ShouldReturnExpectedResult(string? inputString, string? expectedResult)
    {
        string vbString = ConvertToVb6Value(inputString);
        string code = $@"
            Dim result
            result = UCase({vbString})
            Debug.Print result
        ";

        await Run(code);

        Vb6Value expectedValue = expectedResult is { } str ? new Vb6Value(str) : expectedResult is null ? Vb6Value.Null : throw new Exception();

        AssertDebugLog([expectedValue]);
    }

    [Test]
    public async Task BuiltinConstants_HaveOriginalValue()
    {
        string code = $@"
            Debug.Print vbMsgBoxRtlReading
        ";

        await Run(code);

        AssertDebugLog([1048576]);
    }
}