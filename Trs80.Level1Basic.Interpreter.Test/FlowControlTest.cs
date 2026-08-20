using System.Collections.Generic;
using System.IO;
using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class FlowControlTest
{
    [TestMethod]
    public void Interpreter_Can_Execute_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 goto 40",
            "30 i=7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Bare_Next()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR M=32 TO 34",
            "20 PRINT M;M*M;M^3",
            "30 NEXT"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 32  1024  32768 ");
        controller.ReadOutputLine().Should().Be(" 33  1089  35937 ");
        controller.ReadOutputLine().Should().Be(" 34  1156  39304 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_A_MultiCharacter_For_Variable()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR KOUNT=0 TO 3",
            "20 PRINT KOUNT",
            "30 NEXT"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Outer_And_Inner_For_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR OUTER=4 TO 4",
            "20 FOR INNER=4 TO 4",
            "30 PRINT OUTER*INNER",
            "40 NEXT INNER",
            "50 NEXT OUTER"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 16 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multiple_Next_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I=1 TO 2",
            "20 FOR K=1 TO 2",
            "30 PRINT I;K",
            "40 NEXT I,K"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1  1 ");
        controller.ReadOutputLine().Should().Be(" 1  2 ");
        controller.ReadOutputLine().Should().Be(" 2  1 ");
        controller.ReadOutputLine().Should().Be(" 2  2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Toggle_Trace_With_Tron_And_Troff()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 TRON",
            "20 PRINT \"ON\"",
            "30 TROFF",
            "40 PRINT \"OFF\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("(20)ON");
        controller.ReadOutputLine().Should().Be("(30)OFF");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Immediate_Go_To_Into_Loaded_Program()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT \"LINE 10\":END",
            "20 PRINT \"LINE 20\":END"
        });

        controller.ExecuteLine("GO TO 10");

        controller.ReadOutputLine().Should().Be("LINE 10");
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Spaced_Go_To_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A=1",
            "15 B=10",
            "20 GO TO 40",
            "30 PRINT \"LINE 30 A=\";A",
            "40 PRINT \"LINE 40 B=\";B",
            "50 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("LINE 40 B= 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_End()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 end",
            "20 print \"How did I get here?\""
        };

        controller.RunProgram(program);

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Cont_Does_Not_Resume_After_End()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"BEFORE\"",
            "20 END",
            "30 PRINT \"AFTER\""
        };

        controller.RunProgram(program);
        controller.ExecuteLine("CONT");

        controller.ReadOutputLine().Should().Be("BEFORE");
        controller.IsEndOfRun().Should().BeTrue();
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    public void New_Clears_The_Stop_Continuation_Point()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT \"BEFORE\"",
            "20 STOP",
            "30 PRINT \"AFTER\""
        };

        controller.RunProgram(program);
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("CONT");

        controller.ReadOutputLine().Should().Be("BEFORE");
        controller.ReadOutputLine().Should().Be("BREAK IN 20");
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = 7",
            "110 return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
    
    [TestMethod]
    public void Interpreter_Can_Handle_Gosub_Without_Return()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 I=3",
            "20 GOSUB 100",
            "30 PRINT I",
            "40 END",
            "100 I = 7",
            "110 PRINT I",
            "120 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Correct_Statement_After_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3 : gosub 100 : i = i + 1",
            "20 print i",
            "30 end",
            "100 i = 5",
            "110 return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 then 40",
            "30 i = 7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 goto 40",
            "30 i = 7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Print()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 then print i;",
            "30 i = 7",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 5  7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Integer_If()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 0",
            "20 IF A = 0 THEN PRINT \"TRUE\" : END",
            "30 PRINT \"FALSE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Falsey_Implied_Integer_If()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 0",
            "20 IF A THEN PRINT \"TRUE\" : END",
            "30 PRINT \"FALSE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Truthy_Implied_Integer_If()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 1",
            "20 IF A THEN PRINT \"TRUE\" : END",
            "30 PRINT \"FALSE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Variable_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 0 : s = 2",
            "20 if i < 0 then s = -1",
            "30 if i = 0 then s = 0",
            "40 if i > 0 then s = 1",
            "40 print s"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Multiple_Variable_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 4: p=3.14159: e=2.71828",
            "20 if i = 4 then t=p:p=e:e=t",
            "30 print p;e"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2.71828  3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Invalid_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i = 4",
            "20 if i = 4 then 100: goto 200",
            "30 print \"IF is broken\"",
            "40 end",
            "100 print i",
            "110 end",
            "200 print \"THEN is broken\"",
            "210 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n = 1 to 10 : next n",
            "20 print n",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 11 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Without_Next()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I = 1 TO 10",
            "20 PRINT I",
            "30 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop_With_Step()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n = 1 to 10 step 2",
            "20 i = i + n",
            "30 next n",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 25 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop_With_Negative_Step()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for n = 10 to 3 step -1",
            "30 next n",
            "40 print n"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Nested_For_Loop()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 1",
            "30 for i = 1 to 2",
            "40 for j = 1 to 5",
            "50 a = a * 2",
            "60 next j",
            "70 next i",
            "80 print a"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1024 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Goto()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 4",
            "20 on a goto 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: print a: end",
            "200 a = a - 1: print a: end",
            "300 a = a * 2: print a: end",
            "400 a = a / 2: print a: end",
            "500 a = a + 2: print a: end",
            "600 a = a * a: print a: end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Goto_With_Float()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 5.2",
            "20 on a goto 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: print a: end",
            "200 a = a - 1: print a: end",
            "300 a = a * 2: print a: end",
            "400 a = a / 2: print a: end",
            "500 a = a + 2: print a: end",
            "600 a = a * a: print a: end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7.2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Gosub()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a = 4",
            "20 on a gosub 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: return",
            "200 a = a - 1: return",
            "300 a = a * 2: return",
            "400 a = a / 2: return",
            "500 a = a + 2: return",
            "600 a = a * a: return"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Spaced_Go_To()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "5 Z=2",
            "10 ON Z GO TO 30, 50, 40",
            "30 PRINT \"TRANSFER TO LINE 30 WITH Z=\";Z:END",
            "40 PRINT \"TRANSFER TO LINE 40 WITH Z=\";Z:END",
            "50 PRINT \"TRANSFER TO LINE 50 WITH Z=\";Z:END",
            "15 PRINT \"I SHOULDN'T BE HERE.\"",
            "60 PRINT \"I SHOULDN'T BE HERE EITHER.\"",
            "70 END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRANSFER TO LINE 50 WITH Z= 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Immediate_On_Go_To_Into_Loaded_Program()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT \"LINE 10\":END",
            "20 PRINT \"LINE 20\":END",
            "30 PRINT \"LINE 30\":END",
            "40 PRINT \"LINE 40\":END"
        });

        controller.ExecuteLine("Z=2");
        controller.ExecuteLine("ON Z GO TO 30, 40");

        controller.ReadOutputLine().Should().Be("LINE 40");
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Immediate_On_Gosub_Into_Loaded_Program()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "100 PRINT \"SUBROUTINE 100\":RETURN",
            "200 PRINT \"SUBROUTINE 200\":RETURN"
        });

        controller.ExecuteLine("A=2");
        controller.ExecuteLine("ON A GOSUB 100, 200");

        controller.ReadOutputLine().Should().Be("SUBROUTINE 200");
    }

    [TestMethod]
    public void Interpreter_Accepts_On_Error_Go_To_And_Resume()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 ON ERROR GO TO 80",
            "70 ON ERROR GO TO 0",
            "80 PRINT \"WARNING\"",
            "90 RESUME"
        });

        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    public void Interpreter_Handles_Runtime_Error_With_On_Error_Go_To()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON ERROR GO TO 80",
            "20 X=1/0",
            "30 PRINT \"AFTER ERROR\";ERL:END",
            "80 PRINT \"HANDLED\";ERL",
            "90 RESUME"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HANDLED 20 ");
        controller.ReadOutputLine().Should().Be("AFTER ERROR 20 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Disables_On_Error_Handler_With_Zero_Target()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON ERROR GO TO 80",
            "20 X=1/0",
            "30 ON ERROR GO TO 0",
            "40 X=1/0",
            "50 END",
            "80 PRINT \"HANDLED\":RESUME"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HANDLED");
        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Resumes_And_Rearms_On_Error_Handler()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON ERROR GO TO 80",
            "20 X=1/0",
            "30 X=1/0",
            "40 PRINT \"DONE\":END",
            "80 PRINT \"HANDLED\";ERL",
            "90 RESUME"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HANDLED 20 ");
        controller.ReadOutputLine().Should().Be("HANDLED 30 ");
        controller.ReadOutputLine().Should().Be("DONE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Does_Not_Reenter_A_Faulting_On_Error_Handler()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 ON ERROR GO TO 80",
            "20 X=1/0",
            "30 END",
            "80 X=1/0"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Clear_String_Input_Overflow()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("CHRIS J BREISCH\r\n");
        var program = new List<string> {
            "100 CLEAR 12",
            "110 ON ERROR GOTO 200",
            "120 INPUT \"CUSTOMER'S NAME\";N$",
            "130 END",
            "200 IF ERL=120 THEN PRINT \"NAME MUST BE LESS THAN 13 CHARACTERS\":END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CUSTOMER'S NAME? NAME MUST BE LESS THAN 13 CHARACTERS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Integer_Input_Overflow()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("123456789\r\n");
        var program = new List<string> {
            "100 ON ERROR GOTO 200",
            "110 INPUT \"CUSTOMER'S NUMBER\";A%",
            "120 END",
            "200 IF ERL=110 THEN PRINT \"CUSTOMER'S ID NUMBER MUST BE LESS THAN 32768\":END"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CUSTOMER'S NUMBER? CUSTOMER'S ID NUMBER MUST BE LESS THAN 32768");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_New_Clears_Erl_And_On_Error_State()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "80 PRINT \"HANDLED\":RESUME"
        });
        controller.ExecuteLine("ON ERROR GO TO 80");
        controller.ExecuteLine("PRINT ERL");
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("PRINT ERL");

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.ReadOutputLine().Should().Be(" 0 ");
    }

    [TestMethod]
    public void Interpreter_Reports_Resume_Without_Active_Error()
    {
        using var controller = new TestController();
        controller.RunProgram(new List<string> {
            "10 RESUME"
        });

        controller.ReadOutputLine().Should().Be("?FC ERROR IN 10");
        controller.ReadErrorLine().Should().Be(" 10  ?RESUME");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Return_After_Then()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0",
            "20 gosub 100",
            "30 print \"SUCCESS!\"",
            "40 end",
            "100 if a = 0 then return",
            "110 print \"FAIL!\"",
            "120 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("SUCCESS!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Executes_Goto_After_Multiple_Then_Statements()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0",
            "20 if a = 0 then a = 1 : goto 100",
            "30 print a",
            "40 end",
            "100 a = 2",
            "110 print a",
            "120 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Always_Executes_For_Loop_At_Least_Once()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 for i = 1 to 0",
            "20 print i",
            "30 next i",
            "40 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1 ");
        controller.ReadOutputLine().Should().Be(" 2 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}
