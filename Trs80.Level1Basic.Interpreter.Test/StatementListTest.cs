using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class StatementListTest
{
    private static IStatement ParseInput(string input)
    {
        INativeFunctions natives = new NativeFunctions();
        IScanner scanner = new Scanner(natives);
        IParser parser = new Parser(natives);

        List<Token> tokens = scanner.ScanTokens(input);
        IStatement statement = parser.Parse(tokens);
        return statement;
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
    public void Can_Create_StatementList()
    {
        var list = new StatementList();

        list.Should().NotBeNull();
    }

    [TestMethod]
    public void Can_Create_A_Single_Statement_Not_As_List()
    {

        string input = "10 i = i + 1";
        IStatement statement = ParseInput(input);
        statement.Should().NotBeOfType<StatementList>();
    }

    [TestMethod]
    public void Can_Add_Two_Statements_To_StatementList()
    {
        string input = "10 i = i + 1 : i = i * 2";
        IStatement statement = ParseInput(input);
        statement.Should().BeOfType<Compound>();
        StatementList? list = (statement as Compound)?.Statements;

        list!.Count.Should().Be(2);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? previousStatement = secondStatement.Previous;

        secondStatement.Should().NotBeNull();
        previousStatement.Should().Be(firstStatement);

    }

    [TestMethod]
    public void Can_Add_Three_Statements_To_StatementList()
    {
        string input = "10 i = i + 1 : i = i * 2 : print i";
        IStatement statement = ParseInput(input);
        statement.Should().BeOfType<Compound>();
        StatementList? list = (statement as Compound)?.Statements;

        list!.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;

        thirdStatement.Should().NotBeNull();
    }
}