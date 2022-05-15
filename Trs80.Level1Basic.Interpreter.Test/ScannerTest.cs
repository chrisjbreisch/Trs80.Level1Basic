using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trs80.Level1Basic.VirtualMachine.Environment;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ScannerTest
{
    [TestMethod]
    public void Scanner_Can_Scan_A_Simple_Line_Correctly()
    {
        string input = "10 print \"Hello, World!\"";

        IBuiltinFunctions builtins = new BuiltinFunctions();
        IScanner scanner = new Scanner(builtins);

        List<Token> tokens = scanner.ScanTokens(input);
        tokens.Should().HaveCount(4);
        tokens[0].Type.Should().Be(TokenType.Number);

        Assert.AreEqual(tokens[0].Literal, 10);

        tokens[1].Type.Should().Be(TokenType.Print);
        tokens[2].Type.Should().Be(TokenType.String);

        Assert.AreEqual(tokens[2].Literal, "Hello, World!");

        tokens[3].Type.Should().Be(TokenType.EndOfLine);
    }
}