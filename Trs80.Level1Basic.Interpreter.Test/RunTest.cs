using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class RunTest
{
    [TestMethod]
    public void Interpreter_Can_Run_Empty_Program()
    {
        using var controller = new TestController();

        controller.ExecuteLine("NEW");
        controller.ExecuteLine("RUN");

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program_With_Only_Data()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 10, 20",
        };

        controller.RunProgram(program);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program_From_Beginning()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"Hello, World!\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO, WORLD!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program_From_Middle()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"Hello, world!\"",
            "20 end",
            "30 print \"Goodbye!\""
        };

        controller.ExecuteLine("new");

        controller.ExecuteStatements(program);

        controller.ExecuteLine("run 30");

        controller.ReadOutputLine().Should().Be("GOODBYE!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program_From_Middle_And_Jump_To_Line_Before_Start()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"Hello, world!\"",
            "20 end",
            "30 goto 10",
            "40 print \"Goodbye!\""
        };

        controller.ExecuteLine("new");

        controller.ExecuteStatements(program);

        controller.ExecuteLine("run 30");

        controller.ReadOutputLine().Should().Be("HELLO, WORLD!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program_After_End()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"HELLO, WORLD!\"",
            "20 END",
        };

        controller.ExecuteLine("NEW");

        controller.ExecuteStatements(program);

        controller.ExecuteLine("RUN 30");

        controller.IsEndOfRun().Should().BeTrue();
    }
}