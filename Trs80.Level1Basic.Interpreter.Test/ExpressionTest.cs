using System.Collections.Generic;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ExpressionTest
{
    [TestMethod]
    public void Interpreter_Can_Execute_Simple_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multiple_Assignments()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "15 i=7",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Simple_Math()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 j=4",
            "30 n=i*j",
            "40 print n"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 12 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Harder_Math()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 c=25",
            "20 f=(9/5) * c + 32",
            "30 print f"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 77 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Addition()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 i= i + 1",
            "30 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Process_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"Chris\"",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO, CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }
}