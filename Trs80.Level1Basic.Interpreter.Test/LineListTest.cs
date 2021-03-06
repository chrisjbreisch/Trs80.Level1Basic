using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class LineListTest
{
    private int _currentLineNumber;
    private const string _line = "I = I + ";

    private string GetNextLine()
    {
        _currentLineNumber += 10;
        return $"{_currentLineNumber} {_line} {_currentLineNumber}";
    }

    private static IStatement ParseInput(string input)
    {
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
        return statement;
    }

    [TestMethod]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
    public void Can_Create_LineList()
    {
        var list = new LineList();

        list.Should().NotBeNull();
    }


    [TestMethod]
    public void Can_Add_A_Single_Statement_To_LineList()
    {
        var list = new LineList();

        string input = GetNextLine();
        IStatement statement = ParseInput(input);
        list.Add(statement);

        list.Count.Should().Be(1);
        list[0].LineNumber.Should().Be(10);
        list[0].SourceLine.Should().Be(statement.SourceLine);
    }

    [TestMethod]
    public void Can_Add_Two_Statements_To_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 2; i++)
        {
            string input = GetNextLine();
            IStatement statement = ParseInput(input);
            list.Add(statement);
        }

        list.Count.Should().Be(2);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = list[1];
        IStatement? previousStatement = secondStatement?.Previous;

        secondStatement.Should().NotBeNull();
        secondStatement?.LineNumber.Should().Be(20);
        previousStatement.Should().Be(firstStatement);

    }

    [TestMethod]
    public void Can_Add_Three_Statements_To_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string input = GetNextLine();
            IStatement statement = ParseInput(input);
            list.Add(statement);
        }

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = list[1];
        IStatement? thirdStatement = list[2];

        firstStatement.Should().NotBeNull();
        firstStatement.LineNumber.Should().Be(10);
        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
    }

    [TestMethod]
    public void Can_Add_A_Statement_In_The_Middle_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 2; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string input = $"15 {_line}15";
        IStatement statement = ParseInput(input);
        list.Add(statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = list[1];
        IStatement? thirdStatement = list[2];

        firstStatement.Should().NotBeNull();
        firstStatement.LineNumber.Should().Be(10);

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(15);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(20);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_In_The_Middle_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "I = I * 2";
        string input = $"20 {newStatement}";
        IStatement statement = ParseInput(input);
        list.Replace(statement.LineNumber, statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = list[1];
        IStatement? thirdStatement = list[2];

        firstStatement.Should().NotBeNull();
        firstStatement.LineNumber.Should().Be(10);

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);
        secondStatement.SourceLine.Should().Be(newStatement);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_At_End_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "I = I * 2";
        string input = $"30 {newStatement}";
        IStatement statement = ParseInput(input);
        list.Replace(statement.LineNumber, statement);

        list.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = list[1];
        IStatement? thirdStatement = list[2];

        firstStatement.Should().NotBeNull();
        firstStatement.LineNumber.Should().Be(10);

        secondStatement.Should().NotBeNull();
        secondStatement.LineNumber.Should().Be(20);

        thirdStatement.Should().NotBeNull();
        thirdStatement.LineNumber.Should().Be(30);
        thirdStatement.SourceLine.Should().Be(newStatement);
        thirdStatement.Next.Should().BeNull();
        thirdStatement.Previous.Should().Be(secondStatement);
    }

    [TestMethod]
    public void Can_Replace_A_Statement_At_Beginning_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
            list.Add(forStatement);
        }

        string newStatement = "I = I * 2";
        string input = $"10 {newStatement}";
        IStatement statement = ParseInput(input);
        list.Replace(statement.LineNumber, statement);

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
    public void Can_Remove_A_Statement_In_The_Middle_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
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
    public void Can_Remove_A_Statement_At_End_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
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
    public void Can_Remove_A_Statement_At_Beginning_Of_LineList()
    {
        var list = new LineList();
        for (int i = 0; i < 3; i++)
        {
            string forInput = GetNextLine();
            IStatement forStatement = ParseInput(forInput);
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