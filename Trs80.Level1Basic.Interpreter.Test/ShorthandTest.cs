using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ShorthandTest
{
    [TestMethod]
    public void P_Is_Shorthand_For_Print()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 p.\"hello\"",
        };

        controller.RunProgram(program);
        
        controller.ReadOutputLine().Should().Be("HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void N_Is_Shorthand_For_New()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };

        controller.ExecuteStatements(program);
        controller.ExecuteLine("n.");
        controller.ExecuteLine("print mem");
        
        controller.ReadOutputLine().Should().Be(" 15871 ");
    }

    [TestMethod]
    public void R_Is_Shorthand_For_Run()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };

        controller.ExecuteStatements(program);
        controller.ExecuteLine("r.");

        controller.ReadOutputLine().Should().Be("HELLO");
    }

    [TestMethod]
    public void L_Is_Shorthand_For_List()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };

        controller.ExecuteStatements(program);
        controller.ExecuteLine("l.");

        controller.ReadOutputLine().Should().Be(" 10  PRINT \"HELLO\"");
    }

    [TestMethod]
    public void E_Is_Shorthand_For_End()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 e.",
            "30 print i"
        };

        controller.RunProgram(program);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void T_Is_Shorthand_For_Then()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 if i=3 t. print i:end",
            "30 print i+1"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void G_Is_Shorthand_For_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 g.100",
            "30 print i",
            "40 end",
            "100 print i+1"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void In_Is_Shorthand_For_Input()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Chris");

        var program = new List<string> {
            "10 in. \"Enter your name\";A$",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ENTER YOUR NAME?HELLO, CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void M_Is_Shorthand_For_Mem()
    {
        using var controller = new TestController();

        controller.ExecuteLine("new");
        controller.ExecuteLine("p.m.");

        controller.ReadOutputLine().Should().Be(" 15871 ");
    }

    [TestMethod]
    public void F_Is_Shorthand_For_For()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 f.n=1 to 10",
            "30 i=i*2",
            "40 next n",
            "50 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void N_Is_Shorthand_For_Next()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10",
            "30 i=i*2",
            "40 n. n",
            "50 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void S_Is_Shorthand_For_Step()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10 s. 2",
            "30 i=i*2",
            "40 next n",
            "50 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 32 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void St_Is_Shorthand_For_Stop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 st.",
            "30 print i",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("BREAK AT 20");
    }

    [TestMethod]
    public void C_Is_Shorthand_For_Cont()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 stop",
            "30 print i",
        };

        controller.RunProgram(program);
        controller.ExecuteLine("c.");

        controller.ReadOutputLine().Should().Be("BREAK AT 20");
        controller.ReadOutputLine().Should().Be("");
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Gos_Is_Shorthand_For_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 gos.100",
            "30 print i",
            "40 end",
            "100 i = i * 2",
            "110 return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Ret_Is_Shorthand_For_Return()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = i * 2",
            "110 ret."
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Rea_Is_Shorthand_For_Read()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 rea. a,b,c,d,e",
            "30 print e;d;c;b;a",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void D_Is_Shorthand_For_Data()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 d. 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 print e;d;c;b;a",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Rest_Is_Shorthand_For_Restore()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 rest.",
            "40 read f,g,h,i,j",
            "50 print j;i;h;g;f"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Pa_Is_Shorthand_For_Print_At()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 p.a.200, \"hello\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.Trs80.CursorY.Should().Be(200 / 64 + 3);
        controller.Trs80.CursorX.Should().Be(0);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Handle_Extreme_Shorthand()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "0i=3:ifi=3gos.2:f.n=1to10:i=i*2:n.n:g.4",
            "1p.\"BAD\":e.",
            "2i=1:ret.",
            "3p.\"WRONG\":e.",
            "4p.i:e.",
            "5p.\"FAIL\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}