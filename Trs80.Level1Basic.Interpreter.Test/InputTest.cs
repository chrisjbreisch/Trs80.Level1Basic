using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class InputTest
{
    [TestMethod]
    public void Interpreter_Can_Get_And_Set_Input()
    {
        using var controller = new TestController();
        var input = new StringReader("Chris");
        controller.Input = input;

        controller.Input.Should().BeSameAs(input);
    }

    [TestMethod]
    public void Interpreter_Can_Read_From_Mocked_Console()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Chris");

        var program = new List<string> {
            "10 input \"Enter your name\";A$",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ENTER YOUR NAME?HELLO, CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Input_Into_Array()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("3.14159");

        var program = new List<string> {
            "10 input \"Enter PI\";A(3)",
            "20 print A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().EndWith(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Input_Type()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("CHRIS\r\n3.14159");

        var program = new List<string> {
            "10 INPUT A",
            "20 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine();
        controller.ReadOutputLine().Should().EndWith(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Handle_Input_With_Comma()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Chris");

        var program = new List<string> {
            "10 input \"Enter your name\",A$",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ENTER YOUR NAME?HELLO, CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input1()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Y");

        var program = new List<string> {
            "10 y=1: n=0",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 1 then 100",
            "40 print \"You entered 'NO'\"",
            "50 end",
            "100 print \"You entered 'YES'\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ENTER (Y/N)?YOU ENTERED 'YES'");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input2()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("N");

        var program = new List<string> {
            "10 y=1: n=0",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 1 then 100",
            "40 print \"You entered 'NO'\"",
            "50 end",
            "100 print \"You entered 'YES'\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ENTER (Y/N)?YOU ENTERED 'NO'");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input3()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Z");

        var program = new List<string> {
            "10 y=2: n=1",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 2 then 100",
            "40 if a = 1 then 200",
            "50 print \"You didn't enter 'Y' or 'N'\"",
            "60 end",
            "100 print \"You entered 'YES'\"",
            "110 end",
            "200 print \"You entered 'NO'\"",
            "210 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ENTER (Y/N)?YOU DIDN'T ENTER 'Y' OR 'N'");
        controller.IsEndOfRun().Should().BeTrue();
    }
}