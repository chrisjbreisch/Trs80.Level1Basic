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

    [TestMethod]
    public void Conditional_Branch_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=3",
            "20 IF A=3 THEN PRINT \"YES\":GOTO 40",
            "30 PRINT \"NO\"",
            "40 PRINT \"DONE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("YES");
        controller.ReadOutputLine().Should().Be("DONE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Descending_For_Loop_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I=3 TO 1 STEP -1",
            "20 PRINT I;",
            "30 NEXT I"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  2  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Two_Dimensional_Array_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DIM A(2,2)",
            "20 A(1,2)=42",
            "30 PRINT A(1,2)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 42 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Typed_String_Array_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFSTR A",
            "20 DIM A$(2)",
            "30 A$(1)=\"HELLO\"",
            "40 PRINT A$(1);A$(2)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Double_Precision_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFDBL A-B",
            "20 A=1",
            "30 B=3",
            "40 PRINT A/B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0.3333333 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Integer_Declaration_Casting_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFINT A",
            "20 A=3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Single_Declaration_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFSNG A",
            "20 A=3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void String_Declaration_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFSTR A",
            "20 A=\"CHRIS\"",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Ranged_Integer_Declaration_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFINT A-C",
            "20 A=1.9:B=2.9:C=3.9",
            "30 PRINT A;B;C"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  2  3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void On_Goto_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON 2 GOTO 100,200",
            "20 PRINT \"DONE\":END",
            "100 PRINT \"ONE\":END",
            "200 PRINT \"TWO\":END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TWO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Gosub_Return_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 GOSUB 100",
            "20 PRINT \"DONE\":END",
            "100 PRINT \"SUB\":RETURN"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("SUB");
        controller.ReadOutputLine().Should().Be("DONE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Stop_Cont_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"BEFORE\"",
            "20 STOP",
            "30 PRINT \"AFTER\""
        };

        controller.RunProgram(program);
        controller.ExecuteLine("CONT");

        controller.ReadOutputLine().Should().Be("BEFORE");
        controller.ReadOutputLine().Should().Be("BREAK AT 20");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().Be("AFTER");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Run_From_Line_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"SKIP\"",
            "20 PRINT \"RUN\""
        };

        controller.ExecuteLine("NEW");
        controller.ExecuteStatements(program);
        controller.ExecuteLine("RUN 20");

        controller.ReadOutputLine().Should().Be("RUN");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void New_Clears_Program_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT \"OLD\"");
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("RUN");

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Save_Load_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 80");
        controller.ExecuteLine("SAVE \"save.bas\"");
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("LOAD \"save.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Saved \"save.bas\".");
        controller.ReadOutputLine().Should().Be("Loaded \"save.bas\".");
        controller.ReadOutputLine().Should().Be(" 80 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Merge_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("MERGE \"merge.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Merged \"merge.bas\".");
        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Delete_Range_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("DELETE 10-20");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 30 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void List_Range_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("LIST 20-30");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().Be(" 30  PRINT 30");
    }

    [TestMethod]
    public void List_Open_Ended_Range_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("LIST -20");

        controller.ReadOutputLine().Should().Be(" 10  PRINT 10");
        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
    }

    [TestMethod]
    public void List_From_Line_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("LIST 20-");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().Be(" 30  PRINT 30");
    }

    [TestMethod]
    public void Delete_From_Line_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("DELETE 20-");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Delete_Through_Line_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("DELETE -20");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 30 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void List_Reversed_Range_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("LIST 30-20");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().Be(" 30  PRINT 30");
    }

    [TestMethod]
    public void Delete_Reversed_Range_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("30 PRINT 30");
        controller.ExecuteLine("DELETE 30-20");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Delete_Abbreviation_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("20 PRINT 20");
        controller.ExecuteLine("DEL. 10");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 20 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Load_Abbreviation_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("LO. \"load.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Save_Abbreviation_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 80");
        controller.ExecuteLine("SA. \"save.bas\"");
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("LO. \"save.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Saved \"save.bas\".");
        controller.ReadOutputLine().Should().Be("Loaded \"save.bas\".");
        controller.ReadOutputLine().Should().Be(" 80 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Merge_Abbreviation_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 A=10");
        controller.ExecuteLine("ME. \"merge.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Merged \"merge.bas\".");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Run_Abbreviation_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT 10");
        controller.ExecuteLine("R.");

        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Stop_Abbreviation_Compatibility_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("10 PRINT \"BEFORE\"");
        controller.ExecuteLine("20 ST.");
        controller.ExecuteLine("30 PRINT \"AFTER\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("BEFORE");
        controller.ReadOutputLine().Should().Be("BREAK AT 20");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Restore_Line_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 1,2,3",
            "20 READ A",
            "30 RESTORE 10",
            "40 READ B",
            "50 PRINT A;B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Double_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFSNG A",
            "20 A=1/3",
            "30 PRINT CDBL(A)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0.3333333 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Single_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFDBL A",
            "20 A=1/3",
            "30 PRINT CSNG(A)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" .333333 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Midpoint_Integer_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT CINT(2.5);CINT(-2.5)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 -3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Invalid_Value_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT VAL(\"NO NUMBER\")"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Random_Number_Shape_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 R=RND",
            "20 IF R>=0 AND R<1 THEN PRINT 1 ELSE PRINT 0"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Controlled_Random_Number_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT RND(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Numeric_Base_Format_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT HEX$(255)",
            "20 PRINT OCT$(255)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FF");
        controller.ReadOutputLine().Should().Be("377");
        controller.IsEndOfRun().Should().BeTrue();
    }
}
