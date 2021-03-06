using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class LogicalTest
{
    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions1()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=1",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=0",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions3()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=1",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions4()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=0",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions5()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=1",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions6()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=0",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions7()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=1",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions8()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=0",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_Equality()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3=3",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_Inequality()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3<>4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_LessThan()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3<4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_LessThanOrEqual1()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3<=4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_LessThanOrEqual2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 4<=4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_GreaterThan()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 4>3",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_GreaterThanOrEqual1()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 4>=3",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Truthy_GreaterThanOrEqual2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 4>=4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Falsey_Equality()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3=4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Falsey_Inequality()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3<>3",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Falsey_LessThan()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3<2",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Falsey_LessThanOrEqual()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 3<=2",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Falsey_GreaterThan()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 4>5",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Falsey_GreaterThanOrEqual()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 4>=5",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}