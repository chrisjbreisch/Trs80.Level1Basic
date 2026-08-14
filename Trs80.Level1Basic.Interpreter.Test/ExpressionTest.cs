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

    [TestMethod]
    public void Interpreter_Can_Use_Defint_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A",
            "20 A = 3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defsng_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSNG A",
            "20 A = 3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defdbl_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A",
            "20 A = 3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Alias_After_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 A = \"Chris\"",
            "30 PRINT A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 A = \"Chris\"",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defint_Range_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A-Z",
            "20 A = 3.9",
            "30 B = 4.2",
            "40 PRINT A; B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Range_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A-C",
            "20 A = \"Chris\"",
            "30 B = \"Terry\"",
            "40 PRINT A; B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRISTERRY");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Dim_Statement()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(10)",
            "20 A(3) = 7",
            "30 PRINT A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defint_Declaration_For_Array_Elements()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A",
            "20 DIM A(10)",
            "30 A(3) = 3.9",
            "40 PRINT A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Declaration_For_String_Array_Elements()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 DIM A$(10)",
            "30 A$(3) = \"Chris\"",
            "40 PRINT A$(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }
}