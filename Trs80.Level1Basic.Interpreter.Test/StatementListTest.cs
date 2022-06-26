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
public class StatementListTest
{
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
    public void Can_Create_StatementList()
    {
        var list = new CompoundStatementList(0);

        list.Should().NotBeNull();
    }

    [TestMethod]
    public void Can_Create_A_Single_Statement_Not_As_List()
    {

        string input = "10 i = i + 1";
        IStatement statement = ParseInput(input);
        statement.Should().NotBeOfType<CompoundStatementList>();
    }

    [TestMethod]
    public void Can_Add_Two_Statements_To_StatementList()
    {
        string input = "10 i = i + 1 : i = i * 2";
        IStatement statement = ParseInput(input);
        statement.Should().BeOfType<Compound>();
        CompoundStatementList? list = (statement as Compound)?.Statements;

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
        CompoundStatementList? list = (statement as Compound)?.Statements;

        list!.Count.Should().Be(3);
        list[0].LineNumber.Should().Be(10);
        IStatement? firstStatement = list[0];
        IStatement? secondStatement = firstStatement.Next;
        IStatement? thirdStatement = secondStatement.Next;

        thirdStatement.Should().NotBeNull();
    }
}