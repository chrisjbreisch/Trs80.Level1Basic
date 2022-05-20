using System.Collections.Generic;
using System.Linq;

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
    private int _currentLineNumber;
    private readonly string _statement = "i = i + ";

    private string GetNextStatement()
    {
        _currentLineNumber += 10;
        return $"{_currentLineNumber} {_statement} {_currentLineNumber}";
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
        list.Add(statement);

        list.Count.Should().Be(1);
        list[0].LineNumber.Should().Be(10);
        list[0].SourceLine.Should().Be(statement.SourceLine);
    }

    [TestMethod]
    public void Can_Add_Two_Statements_To_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 2; i++)
        {
            string input = GetNextStatement();
            Statement statement = ParseInput(input);
            list.Add(statement);
        }

        list.Count.Should().Be(2);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? previousStatement = secondStatement.Previous;

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        previousStatement.Should().Be(firstStatement);

    }

    [TestMethod]
    public void Can_Add_Three_Statements_To_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string input = GetNextStatement();
            Statement statement = ParseInput(input);
            list.Add(statement);
        }

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
    }

    [TestMethod]
    public void Can_Add_A_Statement_In_The_Middle_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 2; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string input = $"15 {_statement}";
        Statement statement = ParseInput(input);
        list.AddOrReplace(statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;
        IStatement? indexedSecondStatement = list[1];

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(15);
        indexedSecondStatement.Should().Be(secondStatement);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(20);
    }
    [TestMethod]
    public void Can_Replace_A_Statement_In_The_Middle_Of_StatementList_With_Indexer()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "i = i * 2";
        string input = $"20 {newStatement}";
        Statement statement = ParseInput(input);
        list[1] = statement;

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;
        IStatement? indexedStatement = list[1];

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        secondStatement.SourceLine.Should().Be(newStatement);
        indexedStatement.Should().Be(secondStatement);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_In_The_Middle_Of_StatementList_With_Wrong_Indexer()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "i = i * 2";
        string input = $"20 {newStatement}";
        Statement statement = ParseInput(input);
        list[2] = statement;

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;
        IStatement? indexedStatement = list[1];

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        secondStatement.SourceLine.Should().Be(newStatement);
        indexedStatement.Should().Be(secondStatement);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_In_The_Middle_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "i = i * 2";
        string input = $"20 {newStatement}";
        Statement statement = ParseInput(input);
        list.AddOrReplace(statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;
        IStatement? indexedSecondStatement = list[1];

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        secondStatement.SourceLine.Should().Be(newStatement);
        indexedSecondStatement.Should().Be(secondStatement);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_At_End_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "i = i * 2";
        string input = $"30 {newStatement}";
        Statement statement = ParseInput(input);
        list.AddOrReplace(statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;
        IStatement? indexedStatement = list[2];

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
        thirdStatement.SourceLine.Should().Be(newStatement);
        thirdStatement.Next.Should().BeNull();
        indexedStatement.Should().Be(thirdStatement);
        thirdStatement.Previous.Should().Be(secondStatement);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_At_Beginning_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "i = i * 2";
        string input = $"10 {newStatement}";
        Statement statement = ParseInput(input);
        list.AddOrReplace(statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? indexedStatement = list[0];

        firstStatement.Should().NotBeNull();
        firstStatement.LineNumber.Should().Be(10);
        firstStatement.SourceLine.Should().Be(newStatement);
        indexedStatement.Should().Be(firstStatement);
        secondStatement.Previous.Should().Be(firstStatement);
    }

    [TestMethod]
    public void Can_Remove_A_Statement_In_The_Middle_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        list.Remove(list[1]);

        list.Count.Should().Be(2);
        list[1].LineNumber.Should().Be(30);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? indexedSecondStatement = list[1];

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(30);
        secondStatement.Next.Should().BeNull();
        indexedSecondStatement.Should().Be(secondStatement);
    }

    [TestMethod]
    public void Can_Remove_A_Statement_At_End_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        list.Remove(list[2]);

        list.Count.Should().Be(2);
        list[1].LineNumber.Should().Be(20);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? indexedStatement = list[1];

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        secondStatement.Next.Should().BeNull();
        indexedStatement.Should().Be(secondStatement);
        secondStatement.Previous.Should().Be(firstStatement);
    }

    [TestMethod]
    public void Can_Remove_A_Statement_At_Beginning_Of_StatementList()
    {
        var list = new StatementList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextStatement();
            Statement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        list.Remove(list[0]);

        list.Count.Should().Be(2);
        list[0].LineNumber.Should().Be(20);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        
        firstStatement.Should().NotBeNull();
        firstStatement.LineNumber.Should().Be(20);
        firstStatement.Previous.Should().BeNull();
        secondStatement.Previous.Should().Be(firstStatement);
    }
}

