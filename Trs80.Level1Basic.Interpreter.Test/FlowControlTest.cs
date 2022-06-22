using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class FlowControlTest
{
    [TestMethod]
    public void Interpreter_Can_Execute_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 goto 40",
            "30 i=7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_End()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 end",
            "20 print \"How did I get here?\""
        };

        controller.RunProgram(program);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = 7",
            "110 return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Correct_Statement_After_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3 : gosub 100 : i = i + 1",
            "20 print i",
            "30 end",
            "100 i = 5",
            "110 return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 then 40",
            "30 i = 7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 goto 40",
            "30 i = 7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Print()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 then print i;",
            "30 i = 7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Variable_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 0 : s = 2",
            "20 if i < 0 then s = -1",
            "30 if i = 0 then s = 0",
            "40 if i > 0 then s = 1",
            "40 print s"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Multiple_Variable_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 4: p=3.14159: e=2.71828",
            "20 if i = 4 then t=p:p=e:e=t",
            "30 print p;e"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2.71828  3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Invalid_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 4",
            "20 if i = 4 then 100: goto 200",
            "30 print \"IF is broken\"",
            "40 end",
            "100 print i",
            "110 end",
            "200 print \"THEN is broken\"",
            "210 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n = 1 to 10 : next n",
            "20 print n",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 11 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop_With_Step()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n = 1 to 10 step 2",
            "20 i = i + n",
            "30 next n",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 25 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop_With_Negative_Step()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n = 10 to 3 step -1",
            "30 next n",
            "40 print n"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Nested_For_Loop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 1",
            "30 for i = 1 to 2",
            "40 for j = 1 to 5",
            "50 a = a * 2",
            "60 next j",
            "70 next i",
            "80 print a"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 4",
            "20 on a goto 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: print a: end",
            "200 a = a - 1: print a: end",
            "300 a = a * 2: print a: end",
            "400 a = a / 2: print a: end",
            "500 a = a + 2: print a: end",
            "600 a = a * a: print a: end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Goto_With_Float()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 5.2",
            "20 on a goto 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: print a: end",
            "200 a = a - 1: print a: end",
            "300 a = a * 2: print a: end",
            "400 a = a / 2: print a: end",
            "500 a = a + 2: print a: end",
            "600 a = a * a: print a: end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7.2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 4",
            "20 on a gosub 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: return",
            "200 a = a - 1: return",
            "300 a = a * 2: return",
            "400 a = a / 2: return",
            "500 a = a + 2: return",
            "600 a = a * a: return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Return_After_Then()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0",
            "20 gosub 100",
            "30 print \"SUCCESS!\"",
            "40 end",
            "100 if a = 0 then return",
            "110 print \"FAIL!\"",
            "120 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("SUCCESS!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Goto_After_Multiple_Then_Statements()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0",
            "20 if a = 0 then a = 1 : goto 100",
            "30 print a",
            "40 end",
            "100 a = 2",
            "110 print a",
            "120 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Always_Executes_For_Loop_At_Least_Once()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for i = 1 to 0",
            "20 print i",
            "30 next i",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}