using System.Collections.Generic;
using System.IO;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class RecursionTest
{
    private readonly List<string> _program = new() {
        "10 input \"Enter fibonacci sequence number\"; n",
        "20 gosub 1000",
        "30 print \"The fibonacci number at position\"; n;\"is\"; a(n)",
        "40 end",
        "1000 a(0) = 0 : a(1) = 1 : a(2) = 1",
        "1010 if n <= 2 then return",
        "1020 n = n - 1",
        "1030 gosub 1010",
        "1040 n = n + 1",
        "1050 a(n) = a(n-1) + a(n-2)",
        "1070 return",
    };

    [TestMethod]
    public void Interpreter_Can_Do_0th_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("0");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_1st_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("1");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_2nd_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("2");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_3rd_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("3");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_4th_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("4");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_5th_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("5");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_6th_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("6");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 8 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_15th_Fibonacci()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("15");

        controller.RunProgram(_program);

        controller.ReadOutputLine().Should().EndWith(" 610 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}