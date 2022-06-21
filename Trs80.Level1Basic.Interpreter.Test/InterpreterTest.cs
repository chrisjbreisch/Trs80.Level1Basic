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
}