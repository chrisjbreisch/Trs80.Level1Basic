using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;


namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void Parser_Can_Parse_A_Simple_Line_Correctly()
    {
        string input = "10 print \"Hello, World!\"";

        INativeFunctions natives = new NativeFunctions();
        IHost host = new FakeHost();
        IScanner scanner = new Scanner(host, natives);
        IParser parser = new Parser(host, natives);

        var sourceLine = new SourceLine(input);
        List<Token> tokens = scanner.ScanTokens(sourceLine);
        IStatement statement = parser.Parse(tokens);

        statement.LineNumber.Should().Be(10);
        statement.SourceLine.Should().Be("PRINT \"HELLO, WORLD!\"");

        var printStatement = statement as Print;
        printStatement.Should().NotBeNull();
        printStatement!.Expressions.Count.Should().Be(1);

        var literal = printStatement.Expressions[0] as Literal;
        literal.Should().NotBeNull();
        string value = literal!.Value;
        value.Should().Be("Hello, World!");
        value = literal!.UpperValue;
        value.Should().Be("HELLO, WORLD!");
    }
}