using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class Level2CompatibilityTest
{
    [TestMethod]
    public void Hardware_Statements_Preserve_Graphics_And_Memory_Behavior()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 SET 23,20",
            "15 RESET 23,20",
            "20 POKE 100,65",
            "30 PRINT POINT(23,20);PEEK(100)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0  65 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Typed_Array_And_Mid_Assignment_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFINT A",
            "20 DIM A(2)",
            "30 A(1)=42",
            "40 B$=\"HELLO\"",
            "50 MID$(B$,2,2)=\"AI\"",
            "60 PRINT A(1);B$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 42 HAILO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Binary_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT CVI(\"AB\")",
            "20 PRINT MKI$(16961)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 16961 ");
        controller.ReadOutputLine().Should().Be("AB");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Floating_Binary_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT MKS$(CVS(\"ABCD\"))",
            "20 PRINT MKD$(CVD(\"ABCDEFGH\"))"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ABCD");
        controller.ReadOutputLine().Should().Be("ABCDEFGH");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void User_Function_And_Loop_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEF FNSQUARE(X)=X*X",
            "20 FOR I=1 TO 3",
            "30 PRINT FNSQUARE(I);",
            "40 NEXT I"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  4  9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void On_Gosub_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON 2 GOSUB 100,200",
            "20 PRINT \"DONE\":END",
            "100 PRINT \"ONE\":RETURN",
            "200 PRINT \"TWO\":RETURN"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TWO");
        controller.ReadOutputLine().Should().Be("DONE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Data_Read_Restore_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 5,4,3",
            "20 READ A,B,C",
            "30 PRINT A;B;C",
            "40 RESTORE",
            "50 READ D",
            "60 PRINT D"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  4  3 ");
        controller.ReadOutputLine().Should().Be(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void String_Inspection_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"HELLO WORLD\"",
            "20 PRINT LEFT$(A$,5);RIGHT$(A$,5)",
            "30 PRINT INSTR(A$,\"WORLD\");VAL(\" 123.4XYZ\")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLOWORLD");
        controller.ReadOutputLine().Should().Be(" 7  123.4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Math_And_Formatting_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT SQR(9);ABS(-4);SGN(-2)",
            "20 PRINT UCASE$(\"hello\");STRING$(3,\"!\")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  4 -1 ");
        controller.ReadOutputLine().Should().Be("HELLO!!!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Character_And_Substring_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"HELLO\"",
            "20 PRINT MID$(A$,2,3);CHR$(33);ASC(\"A\");LEN(A$)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ELL! 65  5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Logical_Operator_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 1 AND 1;1 OR 0;NOT 0",
            "20 PRINT 1 XOR 0;1 EQV 1;0 IMP 1"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  1  1 ");
        controller.ReadOutputLine().Should().Be(" 1  1  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Numeric_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT CINT(2.6);FIX(-2.6);INT(-2.6)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 -2 -3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Optional_String_Argument_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"HELLO\"",
            "20 PRINT INSTR(3,A$,\"L\");MID$(A$,2,2)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 EL");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Input_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("CHRIS");
        var program = new List<string> {
            "10 INPUT \"NAME\";A$",
            "20 PRINT \"HELLO, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("NAME?HELLO, CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Date_And_Time_Shape_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT LEN(DATE$);LEN(TIME$)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 8  8 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Spc_And_Tab_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"A\";SPC(2);\"B\";TAB(8);\"C\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("A  B    C");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Clear_Preserves_Program_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=42",
            "20 DIM B(1)",
            "30 B(1)=7",
            "40 CLEAR",
            "50 PRINT A;B(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0  0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}
