using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class NativeFunctionTest
{
    [TestMethod]
    public void Interpreter_Can_Call_Abs()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=-3.14:b=abs(a):c=a.(b)",
            "20 print a;b;c"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14  3.14  3.14 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Abs_With_Double_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFDBL A",
            "20 A = -3.9",
            "30 PRINT ABS(A)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Abs_With_Int_Value()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=-3.14:b=abs(int(a)):c=a.(b)",
            "20 print a;b;c"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14  4  4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Chr()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print chr$(34);\"hello\";chr$(34)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("\"HELLO\"");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Chr_With_Non_Integer_Argument()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print chr$(65.5)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("A");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Int()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=-3.14:b=int(a):c=3.14:d=i.(c)",
            "20 print a;b;c;d"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.14 -4  3.14  3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Int_Without_Losing_Double_Precision()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFDBL A",
            "20 A = 16777215",
            "30 A = A + 0.9",
            "40 PRINT INT(A) - 16777215"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_String_Functions()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a$ = \"HELLO WORLD\"",
            "20 print left$(a$, 5); \" \"; right$(a$, 5); \" \"; mid$(a$, 7, 5)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO WORLD WORLD");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Len_And_Asc()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print len(\"HELLO\"); asc(\"H\")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  72 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Val_And_Str()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print val(\"123\") + 1",
            "20 print str$(123)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 124 ");
        controller.ReadOutputLine().Should().Be("123");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Instr_And_Space()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print instr(\"HELLO WORLD\", \"WORLD\"); len(space$(3))"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7  3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_String_Function()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print string$(3, \"X\"); len(string$(3, \"AB\"))"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("XXX 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Hex_And_Oct()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print hex$(255)",
            "20 print oct$(255)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FF");
        controller.ReadOutputLine().Should().Be("377");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Sgn()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print sgn(-3);sgn(0);sgn(5)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-1  0  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Cint()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print cint(-3.9); cint(3.4); cint(5.6)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-4  3  6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Fix()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print fix(-3.9); fix(3.4); fix(5.6)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3  3  5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Fix_Without_Losing_Double_Precision()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFDBL A",
            "20 A = 16777215",
            "30 A = A + 0.9",
            "40 PRINT FIX(A) - 16777215"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Csng()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print csng(3.5)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Cint_Without_Losing_Double_Precision()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFDBL A",
            "20 A = 16777216",
            "30 A = A + 0.6",
            "40 PRINT CINT(A) - 16777217"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Cdbl()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print cdbl(3.5)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Lcase()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print lcase$(\"HELLO\")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("hello");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Ucase()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print ucase$(\"hello\")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Trim()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print trim$(\"  hello  \")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Ltrim_And_Rtrim()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print ltrim$(\"  hello\"); rtrim$(\"hello  \")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLOHELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Peek_And_Poke()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 poke 100,65",
            "20 print peek(100)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 65 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Pos_And_Csrlin()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print pos(0); csrlin"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0  0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Inkey()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print len(inkey$)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Math_Functions()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print sqr(9); sin(0); cos(0); tan(0); atn(1); log(10); exp(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  0  1  0  0.7853982  2.302585  2.718282 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Fre()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print fre(0); mem"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 15850  15850 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Input_String()
    {
        using var controller = new TestController
        {
            Input = new StringReader("abc")
        };
        var program = new List<string> {
            "10 print input$(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ABC");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Date_And_Time()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print len(date$); len(time$)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().MatchRegex(@"^\s*8\s+8\s*$");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Assign_With_Mid()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$ = \"HELLO\"",
            "20 MID$(A$, 2, 2) = \"IP\"",
            "30 PRINT A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HIPLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Assign_With_Mid_Without_Length()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$ = \"HELLO\"",
            "20 MID$(A$, 2) = \"IP\"",
            "30 PRINT A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HIPLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Mid_Assignment_Does_Not_Expand_Target()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$ = \"HELLO\"",
            "20 MID$(A$, 2, 2) = \"WORLD\"",
            "30 PRINT A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HWOLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Mid_Assignment_Ignores_Start_Beyond_Target()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$ = \"HELLO\"",
            "20 MID$(A$, 10, 2) = \"XY\"",
            "30 PRINT A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Mem()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print mem;m.",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 15855  15855 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Set_And_Point()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 set(23,20):s.(46,40)",
            "20 if point(23,20) * p.(46,40) then print \"ON\": end",
            "30 print \"OFF\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("ON");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Reset_And_Point()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 set(23,20):s.(46,40)",
            "15 reset(23,20):r.(46,40)",
            "20 if point(23,20) + p.(46,40) then print \"ON\": end",
            "30 print \"OFF\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("OFF");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Mod_Operator()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print 7 mod 3",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Logical_Operators()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print 1 AND 1; 1 OR 0; NOT 0",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  1  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Xor_Eqv_Imp_Operators()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print 1 XOR 0; 1 EQV 1; 0 IMP 1",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  1  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_Without_Arguments()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_0()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(0)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(2)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 2);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_With_Non_Integer_Argument()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(2.5)"
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 2);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_10()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(10)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 10);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_100()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print rnd(100)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 100);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_0()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(0)",
        };
        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(2)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 2);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_10()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(10)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 10);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_100()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print r.(100)",
        };

        controller.RunProgram(program);

        string? output = controller.ReadOutputLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(1, 100);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Tab()
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
    public void Interpreter_Can_Call_T()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print t.(5); \"hello\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("     HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_String_With_Character_Code()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print string$(3, 65)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("AAA");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Spc()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"A\"; spc(3); \"B\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("A   B");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Tab_With_Non_Integer_Argument()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 print \"A\"; tab(5.5); \"B\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("A    B");
        controller.IsEndOfRun().Should().BeTrue();
    }
}