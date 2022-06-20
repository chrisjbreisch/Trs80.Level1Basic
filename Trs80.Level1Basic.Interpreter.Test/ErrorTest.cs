using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ErrorTest
{
    [TestMethod]
    public void Interpreter_Handles_Invalid_Identifier_In_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT CHRIS",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 WHAT?");
        controller.ReadErrorLine().Should().Be(" 10  PRINT C?HRIS");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Identifier_In_Command()
    {
        using var controller = new TestController();
        const string statement = "PRINT CHRIS";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 0 WHAT?");
    }
    [TestMethod]
    public void Interpreter_Handles_Divide_By_Zero_In_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 1/0",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.ReadErrorLine().Should().Be(" 10  PRINT 1/0?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Divide_By_Zero_In_Command()
    {
        using var controller = new TestController();
        const string statement = "PRINT 1/0";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be("HOW?");
    }
}