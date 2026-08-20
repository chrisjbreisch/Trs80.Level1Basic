using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class ErrorTest
{
    [TestMethod]
    public void Interpreter_Handles_Invalid_Identifier_In_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT CHRIS",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Array_Bound_Error_Position()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DIM A(2)",
            "20 PRINT A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.ReadErrorLine().Should().Be(" 20  PRINT A(3?)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Array_Assignment_Bound_Error_Position()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DIM A(2)",
            "20 A(3) = 1"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.ReadErrorLine().Should().Be(" 20  A(3?) = 1");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Identifier_In_For()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR CHRIS = 1 TO 10",
            "20 NEXT CHRIS"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  FOR C?HRIS = 1 TO 10");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_String_Variable_In_Logical_Expression()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"\"",
            "20 IF A$ THEN PRINT \"TRUE\" : END",
            "30 PRINT \"FALSE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 20");
        controller.ReadErrorLine().Should().Be(" 20  IF A$ ?THEN PRINT \"TRUE\" : END");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Identifier_In_Next()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I = 1 TO 10",
            "20 NEXT CHRIS"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("WHAT?");
        controller.ReadErrorLine().Should().Be(" 20  NEXT C?HRIS");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Unary_Operand()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT -\"3\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("WHAT?");
        controller.ReadErrorLine().Should().Be(" 10  PRINT -?\"3\"");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Mid_Assignment_To_Numeric_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 1",
            "20 MID$(A, 1, 1) = \"X\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 20");
        controller.IsEndOfRun().Should().BeTrue();
    }


    [TestMethod]
    public void Interpreter_Handles_Invalid_Binary_Operands()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"3\" * \"4\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?TM ERROR");
        controller.ReadErrorLine().Should().Be(" 10  PRINT \"3\" ?* \"4\"");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_For_String_And_Numeric_Addition()
    {
        using var controller = new TestController();

        controller.ExecuteLine("DEFSNG A");
        controller.ExecuteLine("DEFSTR S");
        controller.ExecuteLine("S=\"STRING\"");
        controller.ExecuteLine("A=1.234567");
        controller.ExecuteLine("PRINT S+A");

        controller.ReadOutputLine().Should().Be("?TM ERROR");
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_Assigning_String_To_Numeric_Variable()
    {
        using var controller = new TestController();

        controller.ExecuteLine("A=\"STRING\"");

        controller.ReadOutputLine().Should().Be("?TM ERROR");
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_Assigning_Number_To_String_Variable()
    {
        using var controller = new TestController();

        controller.ExecuteLine("A$=42");

        controller.ReadOutputLine().Should().Be("?TM ERROR");
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_Assigning_String_To_Numeric_Array()
    {
        using var controller = new TestController();

        controller.ExecuteLine("A(1)=\"STRING\"");

        controller.ReadOutputLine().Should().Be("?TM ERROR");
    }

    [TestMethod]
    public void Interpreter_Reports_Type_Mismatch_Assigning_Number_To_String_Array()
    {
        using var controller = new TestController();

        controller.ExecuteLine("A$(1)=42");

        controller.ReadOutputLine().Should().Be("?TM ERROR");
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Identifier_In_Command()
    {
        using var controller = new TestController();
        const string statement = "PRINT CHRIS";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 0 ");
    }

    [TestMethod]
    public void Interpreter_Handles_Divide_By_Zero_In_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT 1/0",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.ReadErrorLine().Should().Be(" 10  PRINT 1/0?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Program_Syntax_Error_Reports_The_Program_Line_Number()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("1200,1\r\n1\r\n");
        controller.ExecuteStatements(new List<string> {
            "10 REM PROGRAM COMPUTES MONTHLY MORTGAGE PAYMENTS",
            "15 PRINT \"ENTER THE AMOUNT AND YEARS OF MORTGAGE\"",
            "20 INPUT M,Y",
            "25 PRINT \"ENTER THE INTEREST RATE IN %\"",
            "30 INPUT I",
            "40 N=Y*12",
            "50 I=I/12",
            "60 P=M*I*(I+1)^N/((1+I)^(N-1)"
        });
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("ENTER THE AMOUNT AND YEARS OF MORTGAGE");
        controller.ReadOutputLine().Should().Be("?ENTER THE INTEREST RATE IN %");
        controller.ReadOutputLine().Should().Be("??SN ERROR IN 60");
        controller.PendingEdit.TryTake(out int editLineNumber).Should().BeTrue();
        editLineNumber.Should().Be(60);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Variable_Containing_Reserved_On()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 GONE=7"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Syntax_Error_For_Invalid_Command_Text()
    {
        using var controller = new TestController();

        controller.ExecuteLine("TRS-80");

        controller.ReadOutputLine().Should().Be("?SN ERROR");
    }

    [TestMethod]
    public void Interpreter_Reports_Integer_Suffix_Overflow_At_Use()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 AB%=1234567",
            "20 PRINT AB%"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?OV ERROR");
        controller.ReadErrorLine().Should().Be(" 20  PRINT AB%?");
        controller.ReadErrorLine().Should().Be("[Integer value out of range.]");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Defint_Assignment_Overflow()
    {
        using var controller = new TestController();

        controller.ExecuteLine("DEFINT I");
        controller.ExecuteLine("IT=40000");

        controller.ReadOutputLine().Should().Be("?OV ERROR");
    }

    [TestMethod]
    public void Interpreter_Reports_Negative_Integer_Suffix_Overflow_At_Use()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 AB%=-32769",
            "20 PRINT AB%"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?OV ERROR");
        controller.ReadErrorLine().Should().Be(" 20  PRINT AB%?");
        controller.ReadErrorLine().Should().Be("[Integer value out of range.]");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Single_Suffix_Overflow_At_Use()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A!=1.8E+38",
            "20 PRINT A!"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?OV ERROR");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Double_Suffix_Overflow_At_Use()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A#=1.8E+38",
            "20 PRINT A#"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?OV ERROR");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Integer_Literal_Suffix_Overflow()
    {
        using var controller = new TestController();

        controller.ExecuteLine("PRINT 32768%");

        controller.ReadOutputLine().Should().Be("?OV ERROR");
    }

    [TestMethod]
    public void Interpreter_Handles_Divide_By_Zero_In_Command()
    {
        using var controller = new TestController();
        const string statement = "PRINT 1/0";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be("HOW?");
    }

    [TestMethod]
    public void Interpreter_Handles_Array_Too_Large()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT A(3963)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("SORRY");
        controller.ReadErrorLine().Should().Be(" 10  PRINT A(3963?)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Array_Too_Large_As_Program_Grows()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "5 A = 3",
            "7 B = 7",
            "10 PRINT A(3961)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("SORRY");
        controller.ReadErrorLine().Should().Be(" 10  PRINT A(3961?)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Array_Max_Size()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT A(3962)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
    
    [TestMethod]
    public void Interpreter_Handles_Invalid_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 GOTO 100",
            "20 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?UL ERROR");
        controller.ReadErrorLine().Should().Be(" 10  GOTO 100?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Undefined_Spaced_Goto_Line()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=1",
            "20 GO TO 25",
            "30 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?UL ERROR");
        controller.ReadErrorLine().Should().Be(" 20  GO TO 25?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Reports_Undefined_OnGosub_Line()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=2",
            "20 ON A GOSUB 100, 200",
            "30 END",
            "100 RETURN"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?UL ERROR");
        controller.ReadErrorLine().Should().Be(" 20  ON A GOSUB 100, 200?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }
    
    [TestMethod]
    public void Interpreter_Handles_Invalid_OnGoto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 3",
            "20 ON A GOTO 100, 200, 300",
            "30 END",
            "100 PRINT 100",
            "110 END",
            "200 PRINT 200",
            "210 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?UL ERROR");
        controller.ReadErrorLine().Should().Be(" 20  ON A GOTO 100, 200, 300?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }
    
    [TestMethod]
    public void Interpreter_Handles_Invalid_OnGoto_In_Middle()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 2",
            "20 ON A GOTO 100, 200, 300",
            "30 END",
            "100 PRINT 100",
            "110 END",
            "300 PRINT 300",
            "310 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?UL ERROR");
        controller.ReadErrorLine().Should().Be(" 20  ON A GOTO 100, 200?, 300");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_OnGoto_Out_Of_Range()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 5",
            "20 ON A GOTO 100, 200, 300",
            "30 PRINT 30",
            "40 END",
            "100 PRINT 100",
            "110 END",
            "200 PRINT 200",
            "210 END",
            "300 PRINT 300",
            "310 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 30 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Read()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DATA 10, 20",
            "20 READ"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 20");
        controller.ReadErrorLine().Should().Be(" 20  READ?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_On()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON A",
            "20 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  ON A?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Unexpected_Identifier()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  A?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Bad_Next()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I = 1 TO 10",
            "20 NEXT"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 20");
        controller.ReadErrorLine().Should().Be(" 20  NEXT?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Bad_For_Identifier()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR 3 = 1 TO 10",
            "20 NEXT 3"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  FOR ?3 = 1 TO 10");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Bad_Next_Identifier()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I = 1 TO 10",
            "20 NEXT 3"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 20");
        controller.ReadErrorLine().Should().Be(" 20  NEXT ?3");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }


    [TestMethod]
    public void Interpreter_Handles_Missing_Print_At_Separator()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT AT 3 \"BOO\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("BOO");
        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  PRINT AT 3 ?\"BOO\"");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Truthy_Bad_If()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 1",
            "20 IF A = 1 100",
            "30 PRINT 30",
            "40 END",
            "100 PRINT 100"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 20");
        controller.ReadErrorLine().Should().Be(" 20  IF A = 1 ?100");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Bad_Let()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 LET 3=4",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  LET ?3=4");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Argument_Count_For_Zero_Arg_Function()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=MEM(3)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  A=MEM?(3)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Invalid_Argument_Count_For_Function_With_Args()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=INT(3,4)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  A=INT(3?,4)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Zero_Argument_Count_For_Function_With_Args()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=INT",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  A=INT?");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }
    
    [TestMethod]
    public void Interpreter_Handles_Missing_Arguments()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 SET(3)",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  SET(3?)");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }


    [TestMethod]
    public void Interpreter_Handles_No_Expression()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 +",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  ?+");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Falsey_Bad_If()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 0",
            "20 IF A = 1 100",
            "30 PRINT 30",
            "40 END",
            "100 PRINT 100"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 30 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
    
    [TestMethod]
    public void Interpreter_Handles_On_Without_Redirector()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON A 100, 200, 300",
            "30 PRINT 30",
            "40 END",
            "100 PRINT 100",
            "110 END",
            "200 PRINT 200",
            "210 END",
            "300 PRINT 300",
            "310 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  ON A ?100, 200, 300");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Parser_Reports_Invalid_Line_Number()
    {
        using var controller = new TestController();
        const string statement = "99999 ENd";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be("HOW?");
    }

    [TestMethod]
    public void Scanner_Reports_Invalid_Character()
    {
        using var controller = new TestController();
        const string statement = "10 A{3} = 2";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be("?SN ERROR");
    }
}