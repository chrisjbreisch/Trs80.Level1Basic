using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        IScanner scanner = new Scanner(natives);
        IParser parser = new Parser(natives);

        List<Token> tokens = scanner.ScanTokens(input);
        IStatement statement = parser.Parse(tokens);

        statement.LineNumber.Should().Be(10);
        statement.SourceLine.Should().Be("print \"Hello, World!\"");
        
        var printStatement = ((IListItemDecorator)statement).UnDecorate() as Print;
        printStatement.Should().NotBeNull();
        printStatement!.Expressions.Count.Should().Be(1);

        var literal = printStatement.Expressions[0] as Literal;
        literal.Should().NotBeNull();

        Assert.AreEqual(literal!.Value, "Hello, World!");
    }
}