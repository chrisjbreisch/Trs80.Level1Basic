using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class LonghandSmokeTest
{
    [TestMethod]
    public void Interpreter_Executes_Print()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void New_Clears_Out_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };

        controller.ExecuteStatements(program);
        controller.ExecuteLine("new");
        controller.ExecuteLine("print mem");

        controller.ReadOutputLine().Should().Be(" 15871 ");
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };
        controller.ExecuteStatements(program);
        controller.ExecuteLine("run");

        controller.ReadOutputLine().Should().Be("HELLO");
    }

    [TestMethod]
    public void Interpret_Can_List_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"hello\"",
        };

        controller.ExecuteStatements(program);
        controller.ExecuteLine("list");

        controller.ReadOutputLine().Should().Be(" 10  PRINT \"HELLO\"");
    }

    [TestMethod]
    public void End_Terminates_Program_Execution()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 end",
            "30 print i"
        };

        controller.RunProgram(program);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Then()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 if i=3 then print i:end",
            "30 print i+1"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Goto_Transfers_Execution()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 goto 100",
            "30 print i",
            "40 end",
            "100 print i+1"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Input_Reads_From_User()
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
    public void Print_Mem_Displays_Free_Memory_Size()
    {
        using var controller = new TestController();

        controller.ExecuteLine("new");
        controller.ExecuteLine("print mem");

        controller.ReadOutputLine().Should().Be(" 15871 ");
    }

    [TestMethod]
    public void For_Loops_Execute_Properly()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10",
            "30 i=i*2",
            "40 next n",
            "50 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Step_Controls_Increment_In_For_Loop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10 step 2",
            "30 i=i*2",
            "40 next n",
            "50 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 32 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Step_Can_Be_Negative()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n=10 to 1 step -1",
            "20 print n;",
            "30 next n",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 10  9  8  7  6  5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Stop_Halts_Execution()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=1",
            "20 stop",
            "30 print i",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("BREAK AT 20");
    }

    [TestMethod]
    public void Cont_Resumes_After_Stop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 stop",
            "30 print i",
        };

        controller.RunProgram(program);
        controller.ExecuteLine("cont");

        controller.ReadOutputLine().Should().Be("BREAK AT 20");
        controller.ReadOutputLine().Should().Be("");
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();

    }

    [TestMethod]
    public void Gosub_Calls_Subroutine_And_Returns()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
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
    public void Read_Gets_Values_From_Data_Statement()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 print e;d;c;b;a",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Restore_Resets_Data_Stream_To_Original()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 restore",
            "40 read f,g,h,i,j",
            "50 print j;i;h;g;f"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Strings_Can_Be_Entered_Without_Quotes()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=RADIO SHACK",
            "20 print A$",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("RADIO SHACK");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Strings_Can_Be_Entered_Without_Quotes_On_Multi_Statement_Lines()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=RADIO SHACK:B$=TRS-80 MODEL I",
            "20 print A$;\" \";B$",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("RADIO SHACK TRS-80 MODEL I");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Print_At_Prints_To_A_Position_On_Screen()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print at 200, \"hello\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.Trs80.CursorY.Should().Be(200 / 64 + 3);
        controller.Trs80.CursorX.Should().Be(0);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Handle_Multi_Statement_Lines()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 3 : if i = 3 gosub 100",
            "20 for n = 1 to 10 : i = i * 2 : next n : goto 200",
            "30 print \"BAD\" : end",
            "100 i = 1: return",
            "110 print \"WRONG\" : end",
            "200 print i : end",
            "210 print \"FAIL\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}