using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class InterpreterTest
{
    [TestMethod]
    public void Interpreter_Can_Run_HelloWorld()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"Hello, World!\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO, WORLD!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multi_Statement_Lines()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3 : print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Line_Number_Zero()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "0 let a = 3.14159",
            "1 print a",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Array_Indexes()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A(3) = 3.14159",
            "20 PRINT A(3)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Array_Indexes_With_Reassign()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A(3) = 3",
            "15 A(3) = 3.14159",
            "20 PRINT A(3)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Remarks()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 3",
            "REM A = A * 3",
            "20 A = A * 3",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Apostrophe_Remarks()
    {
        using var controller = new TestController();
        controller.ExecuteLine("10 ' A = A * 3");
        controller.ExecuteLine("RUN");

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Apostrophe_Remarks_After_A_Statement()
    {
        using var controller = new TestController();
        controller.ExecuteLine("10 PRINT \"BEFORE\":' comment");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("BEFORE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Preserves_Apostrophes_Inside_Strings()
    {
        using var controller = new TestController();
        controller.ExecuteLine("10 PRINT \"DON'T ALTER\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("DON'T ALTER");
        controller.IsEndOfRun().Should().BeTrue();
    }
}