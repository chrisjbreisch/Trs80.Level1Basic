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
    public void Interpreter_Doesnt_Fail_When_Reading_Strings_Into_Number_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 data RADIO SHACK",
            "20 read a",
            "30 print a"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

}