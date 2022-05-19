using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class NativeFunctionTest
{
    [TestMethod]
    public void Interpreter_Can_Call_Abs()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=-3.14:b=abs(a):c=a.(b)",
            "20 print a;b;c"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14  3.14  3.14 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Chr()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print chr$(34);\"hello\";chr$(34)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("\"hello\"");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Int()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=-3.14:b=int(a):c=3.14:d=i.(c)",
            "20 print a;b;c;d"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14 -4  3.14  3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Mem()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print mem;m.",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 15855  15855 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Set_And_Point()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 set(23,20):s.(46,40)",
            "20 if point(23,20) * p.(46,40) then print \"ON\": end",
            "30 print \"OFF\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ON");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Reset_And_Point()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 set(23,20):s.(46,40)",
            "15 reset(23,20):r.(46,40)",
            "20 if point(23,20) + p.(46,40) then print \"ON\": end",
            "30 print \"OFF\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("OFF");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_0()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(0)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(2)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 2);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_10()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(10)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 10);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_100()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(100)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 100);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_0()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(0)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(2)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 2);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_10()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(10)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 10);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_100()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(100)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 100);

        controller.IsEndOfRun().Should().BeTrue();
    }
}