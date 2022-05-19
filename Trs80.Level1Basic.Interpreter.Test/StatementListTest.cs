using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
internal class StatementListTest
{
    private int _currentLineNumber = 0;
    private string _statement = "i = i + 1";

    private string GetNextStatement()
    {
        _currentLineNumber += 10;
        return $"{_currentLineNumber} {_statement}";
    }

    [TestMethod]
    public void Can_Create_StatementList()
    {
        var list = new StatementList();

        list.Should().NotBeNull();
    }


    [TestMethod]
    public void Can_Add_A_Single_Statement_To_StatementList()
    {
        var list = new StatementList();
        string input = GetNextStatement();

        Statement statement = ParseInput(input);

        list.AddEnd(statement);

        list.Count().Should().Be(1);
        list[0].LineNumber.Should().Be(10);
        list[0].SourceLine.Should().Be(statement.SourceLine);
    }

    private static Statement ParseInput(string input)
    {
        INativeFunctions natives = new NativeFunctions();
        IScanner scanner = new Scanner(natives);
        IParser parser = new Parser(natives);

        List<Token> tokens = scanner.ScanTokens(input);
        Statement statement = parser.Parse(tokens);
        return statement;
    }
}

