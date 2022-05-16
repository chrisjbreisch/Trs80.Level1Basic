using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class PrintTest
{

    [TestMethod]
    public void Cls_Resets_CursorX_And_CursorY()
    {
        using var controller = new TestController();

        controller.ExecuteLine("print \"hello\"");
        controller.Trs80.CursorY.Should().NotBe(0);
        controller.Trs80.CursorX.Should().Be(0);

        controller.ExecuteLine("cls");
        controller.Trs80.CursorX.Should().Be(0);
        controller.Trs80.CursorY.Should().Be(0);
    }

    [TestMethod]
    public void Interpreter_Can_Print_String()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"Hello, World!\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Hello, World!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Positive_Integer()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Negative_Integer()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -5",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Large_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 1000000",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1E+06 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Strings_Together()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 print a$;b$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("startup");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Strings_With_Comma()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 print a$,b$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("start           up");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Three_Strings_With_Comma()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 C$=\"shutdown\"",
            "40 print a$,b$,c$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("start           up              shutdown");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Four_Strings_With_Comma()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 C$=\"shut\"",
            "40 D$=\"down\"",
            "50 print a$,b$,c$,d$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("start           up              shut            down");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Integers_Together()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 3",
            "20 B = 5",
            "30 print a;b"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Integers_With_Comma()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 3",
            "20 B = 5",
            "30 print a,b"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3               5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Integers_On_Separate_Statements()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 3",
            "20 B = 5",
            "30 print a;",
            "40 print b"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_At_A_Position()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print at 200, \"hello\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("hello");
        controller.Trs80.CursorY.Should().Be(200 / 64 + 3);
        controller.Trs80.CursorX.Should().Be(0);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_With_Tab()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print tab(5); \"hello\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("     hello");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_With_Tabs()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print tab(5); \"hello\";tab(8);\"goodbye\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("     hellogoodbye");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_With_Many_Tabs()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=2:c=3:d=4:e=5:f=6:g=7:h=8:i=9:j=10",
            "20 print a;tab(5);b;tab(10);c;tab(15);d;tab(20);",
            "30 print e;tab(25);f;tab(30);g;tab(35);h;tab(40);",
            "40 print i;tab(45);j"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1    2    3    4    5    6    7    8    9    10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}