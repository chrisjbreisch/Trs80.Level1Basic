using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class DataTest
{
    [TestMethod]
    public void Interpreter_Executes_Data_And_Read()
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
    public void Interpreter_Executes_Data_In_The_Middle()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "20 read a,b,c,d,e",
            "25 data 1,2,3,4,5",
            "30 print e;d;c;b;a",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Data_At_The_End()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "20 read a,b,c,d,e",
            "30 print e;d;c;b;a",
            "40 data 1,2,3,4,5",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Restore()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "5 i = 0",
            "10 data 1, 2, 3, 4, 5",
            "20 for n = 1 to 5",
            "30 read a",
            "40 print a;",
            "50 next n",
            "60 i = i + 1",
            "70 restore",
            "80 if i = 1 then 10",
            "90 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  2  3  4  5  1  2  3  4  5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Clear_Resets_Data_Read_Position()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 1,2",
            "20 READ A",
            "30 PRINT A",
            "40 CLEAR",
            "50 READ A",
            "60 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Out_Of_Data()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 1",
            "20 READ A,B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?OD ERROR");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Read_Strings()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data RADIO SHACK, TRS-80 MODEL I",
            "20 read a$, b$",
            "30 print a$;\" \";b$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("RADIO SHACK TRS-80 MODEL I");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Read_String_Data_Into_Defstr_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFSTR S",
            "20 DATA STRING",
            "30 READ S",
            "40 PRINT S"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("STRING");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_When_Reading_String_Data_Into_Numeric_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data RADIO SHACK",
            "20 read a",
            "30 print a"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?TM ERROR");
        controller.ReadErrorLine().Should().Be(" 20  READ ?A");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_When_Reading_Numeric_Data_Into_String_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 42",
            "20 READ A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?TM ERROR");
        controller.ReadErrorLine().Should().Be(" 20  READ ?A$");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }


    [TestMethod]
    public void Interpreter_Executes_Read_Into_Array()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 for i = 1 to 5",
            "30 read a(i)",
            "40 next i",
            "50 for i = 1 to 5",
            "60 print a(i);",
            "70 next i",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  2  3  4  5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_When_Reading_Numeric_Data_Into_String_Array()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFSTR A",
            "20 DATA 42",
            "30 READ A(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?TM ERROR");
        controller.ReadErrorLine().Should().Be(" 30  READ ?A(1)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_When_Reading_String_Data_Into_Numeric_Array()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA STRING",
            "20 READ A(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?TM ERROR");
        controller.ReadErrorLine().Should().Be(" 20  READ ?A(1)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }
}