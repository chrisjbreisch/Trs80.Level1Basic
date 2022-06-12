using System.Collections.Generic;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class CommandTest
{

    [TestMethod]
    public void Interpreter_Can_Handle_Lines_Inserted_In_The_Middle()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 print i",
            "15 i=i * 2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands1()
    {
        using var controller = new TestController();
        const string statement = "print 3 * 4";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 12 ");
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands2()
    {
        using var controller = new TestController();
        const string statement = "print 345/123";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 2.80488 ");
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands3()
    {
        using var controller = new TestController();
        const string statement = "print (2/3)*(3/2)";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 1 ");
    }

    [TestMethod]
    public void Interpreter_Clears_Memory_Properly()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "print a;b;c;d;e;f;g;h;i;j;k;l;m;n;o;p;q;r;s;t;u;v;w;x;y;z"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine()
            .Should().Be(" 0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0 ");
    }

    [TestMethod]
    public void Interpreter_Returns_Full_Memory_After_New()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "print mem"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 15871 ");
    }

    [TestMethod]
    public void Interpreter_Counts_Static_Memory_Usage1()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "10 a = 25",
            "print mem"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 15861 ");
    }

    [TestMethod]
    public void Interpreter_Counts_Static_Memory_Usage2()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "10 a = 25",
            "20 print \"this example is to measure memory usage.\"",
            "print mem"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 15809 ");
    }

    [TestMethod]
    public void Interpreter_Stores_Variables_Until_Explicitly_Cleared()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 p = 3.14159"
        };

        controller.RunProgram(program);
        controller.ExecuteLine("print p");

        controller.IsEndOfRun().Should().BeTrue();
        controller.ReadOutputLine().Should().Be(" 3.14159 ");
    }
}