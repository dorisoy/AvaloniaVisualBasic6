using System.IO;
using Antlr4.Runtime;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public partial class SyntaxChecker : IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        throw new VBCompileErrorException($"{msg} in line {line}:{charPositionInLine}\nOffending symbol: {offendingSymbol.Text}") { Line = line };
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        throw new VBCompileErrorException($"{msg} in line {line}:{charPositionInLine}") { Line = line };
    }

    public void Run(string code)
    {
        var inputStream = new AntlrInputStream(new StringReader(code));
        var lexer = new VB6Lexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new VB6Parser(commonTokenStream);

        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();

        lexer.AddErrorListener(this);
        parser.AddErrorListener(this);

        var tree = parser.startRule();
    }
}