using System.Collections.Generic;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Application;
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

        var bootstrapper = new Bootstrapper();
        IAppSettings? appSettings = bootstrapper.AppSettings;
        ILoggerFactory? loggerFactory = bootstrapper.LogFactory;

        IHost host = new FakeHost();
        var trs80 = new VirtualMachine.Machine.Trs80(appSettings, loggerFactory, host);
        INativeFunctions natives = new NativeFunctions();
        var scanner = new Scanner(trs80, natives, appSettings);
        var parser = new Parser(trs80, natives, appSettings);

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
        value = literal.UpperValue;
        value.Should().Be("HELLO, WORLD!");
    }

    [TestMethod]
    public void Parser_Accepts_The_Maximum_Program_Line_Number()
    {
        using var controller = new TestController();

        IStatement? statement = controller.Parser.Parse(
            controller.Scanner.ScanTokens(new SourceLine("65529 PRINT 1")));

        statement.Should().NotBeNull();
        statement!.LineNumber.Should().Be(65529);
    }

    [TestMethod]
    public void Parser_Rejects_Reserved_Program_Line_Numbers()
    {
        using var controller = new TestController();

        foreach (int lineNumber in new[] { 65530, 65535 })
        {
            IStatement? statement = controller.Parser.Parse(
                controller.Scanner.ScanTokens(new SourceLine($"{lineNumber} PRINT 1")));

            statement.Should().BeNull();
        }
    }
}