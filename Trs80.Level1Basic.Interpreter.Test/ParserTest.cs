using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Interpreter.Interpreter;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;
using Trs80.Level1Basic.Interpreter.Parser.Statements;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void Parser_Can_Parse_A_Simple_Line_Correctly()
    {
        string input = "10 print \"Hello, World!\"";
        IBuiltinFunctions builtins = new BuiltinFunctions();
        IScanner scanner = new Scanner.Scanner(builtins);
        IParser parser = new Parser.Parser(builtins);

        List<Token> tokens = scanner.ScanTokens(input);
        ParsedLine parsedLine = parser.Parse(tokens);

        parsedLine.LineNumber.Should().Be(10);
        parsedLine.SourceLine.Should().Be("print \"Hello, World!\"");
        parsedLine.Statements.Count.Should().Be(1);

        var printStatement = parsedLine.Statements[0] as Print;
        printStatement.Should().NotBeNull();
        printStatement!.Expressions.Count.Should().Be(1);

        var literal = printStatement.Expressions[0] as Literal;
        literal.Should().NotBeNull();

        Assert.AreEqual(literal!.Value, "Hello, World!");
    }
}