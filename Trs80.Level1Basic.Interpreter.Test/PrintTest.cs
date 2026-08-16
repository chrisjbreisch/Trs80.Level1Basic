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

        controller.ReadOutputLine().Should().Be("HELLO, WORLD!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Positive_Unary_Number()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT +3.12"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.12 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_With_Question_Mark_Alias()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ? 3"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Uses_Fixed_Format_For_Hundredth()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 0.01"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" .01 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Uses_Scientific_Format_For_Thousandth()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 0.001"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1E-03 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Error_After_Closed_Quote()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"HE SAID \"TO BE OR NOT TO BE\"\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("WHAT?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Adjacent_Text_After_Closed_Quote()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"HE SAID, \"I AM HAPPY\"\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("WHAT?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Concatenate_Adjacent_Quoted_Expressions()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"A\"+\"B\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("AB");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Exponent_Operator()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 5^2",
            "20 PRINT 4^0.5"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 25 ");
        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Uses_Right_Associative_Exponentiation()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 2^3^2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 512 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Gives_Exponentiation_Higher_Precedence()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 2*3^2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 18 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Gives_Exponentiation_Precedence_Over_Division()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 8/2^2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Gives_Exponentiation_Precedence_Over_Modulo()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 10 MOD 2^2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Unary_Minus_With_Exponentiation()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT -2^2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Exponentiate_Grouped_Variable_Expressions()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=5",
            "20 PRINT A^2;(A+1)^2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 25  36 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Multi_Character_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 NUMBER=1",
            "20 PRINT NU"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Alphanumeric_String_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 N1$=\"TOBY\"",
            "20 PRINT N1$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TOBY");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Assign_String_Variable_To_String_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 N2$=\"MARION AND ERIC\"",
            "20 N1$=N2$",
            "30 PRINT N1$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("MARION AND ERIC");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Concatenate_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"HELLO\"",
            "20 B$=\" WORLD\"",
            "30 C$=A$+B$+\"!\"",
            "40 PRINT C$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO WORLD!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Applies_Two_Character_Rule_To_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 NUMBER$=\"TOBY\"",
            "20 PRINT NU$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TOBY");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Applies_Two_Character_Rule_To_Alphanumeric_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 NUMBER1=7",
            "20 PRINT NU"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Integer_Type_Suffix()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A%=3.9",
            "20 PRINT A%"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Single_Type_Suffix()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A!=3.9",
            "20 PRINT A!"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Double_Type_Suffix()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A#=1/3",
            "20 PRINT A#"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" .3333333 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Large_Level_Two_Integer_Literal()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 12345678901"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1.23457E+10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Zero()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 I = 0",
            "20 PRINT I"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Float_Zero()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 I = 0.0",
            "20 PRINT I"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
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
    public void Interpreter_Can_Print_Rounded_Large_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 9999999",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1E+07 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Actual_Int()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = INT(999999)",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 999999 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Actual_Large_Int()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = INT(9999999)",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1E+07 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Actual_Large_Rounded_Int()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = INT(98765432.1)",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 9.87654E+07 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Tiny_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 0.0314159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159E-02 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Tiny_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -0.0314159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14159E-02 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Small_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 0.314159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" .314159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Small_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -0.314159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-.314159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_First_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 3.14159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_First_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -3.14159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Second_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 31.4159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 31.4159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Second_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -31.4159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-31.4159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Third_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 314.159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 314.159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Third_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -314.159265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-314.159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Fourth_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 3141.59265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3141.59 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Fourth_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -3141.59265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3141.59 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Fifth_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 31415.9265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 31415.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Fifth_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -31415.9265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-31415.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Sixth_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 314159.265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 314159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Sixth_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -314159.265",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-314159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Seventh_Order_Positive_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 3141592.65",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159E+06 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Seventh_Order_Negative_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = -3141592.65",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14159E+06 ");
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

        controller.ReadOutputLine().Should().Be("STARTUP");
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

        controller.ReadOutputLine().Should().Be("START           UP");
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

        controller.ReadOutputLine().Should().Be("START           UP              SHUTDOWN");
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

        controller.ReadOutputLine().Should().Be("START           UP              SHUT            DOWN");
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

        controller.ReadOutputLine().Should().Be("HELLO");
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

        controller.ReadOutputLine().Should().Be("     HELLO");
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

        controller.ReadOutputLine().Should().Be("     HELLOGOODBYE");
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