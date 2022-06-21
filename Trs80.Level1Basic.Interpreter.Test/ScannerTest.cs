using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ScannerTest
{
    [TestMethod]
    public void Scanner_Can_Scan_A_Simple_Line_Correctly()
    {
        string input = "10 print \"Hello, World!\"";

        INativeFunctions natives = new NativeFunctions();
        IHost host = new FakeHost();
        IScanner scanner = new Scanner(host, natives);

        var sourceLine = new SourceLine(input);
        List<Token> tokens = scanner.ScanTokens(sourceLine);
        tokens.Should().HaveCount(4);
        tokens[0].Type.Should().Be(TokenType.Number);

        int lineNumber = tokens[0].Literal;
        lineNumber.Should().Be(10);

        tokens[1].Type.Should().Be(TokenType.Print);
        tokens[2].Type.Should().Be(TokenType.String);

        string value = tokens[2].Literal;
        value.Should().Be("Hello, World!");

        tokens[3].Type.Should().Be(TokenType.EndOfLine);
    }
}