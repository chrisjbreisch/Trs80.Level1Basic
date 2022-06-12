using System.Collections.Generic;
using System.IO;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class InterpreterTest
{
    [TestMethod]
    public void Interpreter_Can_Run_HelloWorld()
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
    public void Interpreter_Can_Execute_Simple_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multiple_Assignments()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "15 i=7",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Simple_Math()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 j=4",
            "30 n=i*j",
            "40 print n"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 12 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Harder_Math()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 c=25",
            "20 f=(9/5) * c + 32",
            "30 print f"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 77 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

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
    public void Interpreter_Can_Execute_Multi_Statement_Lines()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3 : print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Addition()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 i= i + 1",
            "30 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Handle_Lines_Inserted_In_The_Middle()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 print i",
            "15 i=i * 2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
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
    public void Interpreter_Can_Process_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"Chris\"",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Hello, Chris");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Read_From_Mocked_Console()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Chris");

        var program = new List<string> {
            "10 input \"Enter your name\";A$",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Enter your name?Hello, Chris");
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
    public void Interpreter_Can_Execute_Commands1()
    {
        using var controller = new TestController();
        const string statement = "print 3 * 4";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 12 ");
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands2()
    {
        using var controller = new TestController();
        const string statement = "print 345/123";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 2.80488 ");
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands3()
    {
        using var controller = new TestController();
        const string statement = "print (2/3)*(3/2)";

        controller.ExecuteLine(statement);

        controller.ReadOutputLine().Should().Be(" 1 ");
    }

    [TestMethod]
    public void Interpreter_Clears_Memory_Properly()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "print a;b;c;d;e;f;g;h;i;j;k;l;m;n;o;p;q;r;s;t;u;v;w;x;y;z"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine()
            .Should().Be(" 0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0 ");
    }

    [TestMethod]
    public void Interpreter_Returns_Full_Memory_After_New()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "print mem"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 15871 ");
    }

    [TestMethod]
    public void Interpreter_Counts_Static_Memory_Usage1()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "10 a = 25",
            "print mem"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 15861 ");
    }

    [TestMethod]
    public void Interpreter_Counts_Static_Memory_Usage2()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "new",
            "10 a = 25",
            "20 print \"this example is to measure memory usage.\"",
            "print mem"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 15809 ");
    }

    [TestMethod]
    public void Interpreter_Handles_Line_Number_Zero()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "0 let a = 3.14159",
            "1 print a",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
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
    public void Interpreter_Stores_Variables_Until_Explicitly_Cleared()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 p = 3.14159"
        };

        controller.RunProgram(program);
        controller.ExecuteLine("print p");

        controller.IsEndOfRun().Should().BeTrue();
        controller.ReadOutputLine().Should().Be(" 3.14159 ");
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input1()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Y");

        var program = new List<string> {
            "10 y=1: n=0",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 1 then 100",
            "40 print \"You entered 'NO'\"",
            "50 end",
            "100 print \"You entered 'YES'\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Enter (Y/N)?You entered 'YES'");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input2()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("N");

        var program = new List<string> {
            "10 y=1: n=0",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 1 then 100",
            "40 print \"You entered 'NO'\"",
            "50 end",
            "100 print \"You entered 'YES'\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Enter (Y/N)?You entered 'NO'");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input3()
    {
        using var controller = new TestController();
        controller.Input = new StringReader("Z");

        var program = new List<string> {
            "10 y=2: n=1",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 2 then 100",
            "40 if a = 1 then 200",
            "50 print \"You didn't enter 'Y' or 'N'\"",
            "60 end",
            "100 print \"You entered 'YES'\"",
            "110 end",
            "200 print \"You entered 'NO'\"",
            "210 end"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Enter (Y/N)?You didn't enter 'Y' or 'N'");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Extra_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 C$=\"Chris\"",
            "20 print C$"

        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("Chris");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Extra_Array_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 F(10) = 3.14159",
            "20 print F(10)"

        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
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
    public void Interpreter_Evaluates_Logical_Expressions1()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=1",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions2()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=0",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions3()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=1",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions4()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=0",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions5()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=1",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions6()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=1:b=0",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions7()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=1",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions8()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 a=0:b=0",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("FALSE");
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
    public void Interpreter_Executes_Goto_After_Multiple_Thens()
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