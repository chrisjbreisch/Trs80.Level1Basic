using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class CommandTest
{
    [TestMethod]
    public void Interpreter_Can_Delete_A_Line_With_Delete_Command()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });
        var tokens = controller.Scanner.ScanTokens(new SourceLine("DELETE 10"));
        controller.Parser.Parse(tokens).Should().BeOfType<Delete>();
        controller.ExecuteLine("DELETE 10");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 20 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Delete_A_Line_Range_With_Delete_Command()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });
        controller.ExecuteLine("DELETE 10-20");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 30 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Delete_An_Open_Ended_Line_Range()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });
        controller.ExecuteLine("DELETE -20");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 30 ");

        using var secondController = new TestController();
        secondController.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });
        secondController.ExecuteLine("DELETE 20-");
        secondController.ExecuteLine("RUN");

        secondController.ReadOutputLine().Should().Be(" 10 ");
    }

    [TestMethod]
    public void Interpreter_Can_Delete_With_Abbreviated_Command()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });
        controller.ExecuteLine("DEL. 10");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be(" 20 ");
    }

    [TestMethod]
    public void Parser_Can_Parse_Abbreviated_Load_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("LO. \"PROGRAM.BAS\""));

        controller.Parser.Parse(tokens).Should().BeOfType<Load>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Abbreviated_Save_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("SA. \"PROGRAM.BAS\""));

        controller.Parser.Parse(tokens).Should().BeOfType<Save>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Abbreviated_Merge_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("ME. \"PROGRAM.BAS\""));

        controller.Parser.Parse(tokens).Should().BeOfType<Merge>();
    }

    [TestMethod]
    public void Interpreter_Can_List_A_Line_Range()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });
        var tokens = controller.Scanner.ScanTokens(new SourceLine("LIST 20-30"));
        var parsed = controller.Parser.Parse(tokens).Should().BeOfType<Trs80.Level1Basic.VirtualMachine.Parser.Statements.List>().Subject;
        parsed.EndAtLineNumber.Should().NotBeNull();
        controller.ExecuteLine("LIST 20-30");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().Be(" 30  PRINT 30");
    }

    [TestMethod]
    public void Interpreter_Can_List_An_Open_Ended_Line_Range()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });

        controller.ExecuteLine("LIST -20");
        controller.ReadOutputLine().Should().Be(" 10  PRINT 10");
        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");

        using var secondController = new TestController();
        secondController.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });
        secondController.ExecuteLine("LIST 20-");
        secondController.ReadOutputLine().Should().Be(" 20  PRINT 20");
        secondController.ReadOutputLine().Should().Be(" 30  PRINT 30");
    }

    [TestMethod]
    public void Interpreter_Can_List_A_Reversed_Line_Range()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });
        controller.ExecuteLine("LIST 30-20");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().Be(" 30  PRINT 30");
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
    public void Interpreter_Can_Replace_Line()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 I=3",
            "20 PRINT I",
            "20 PRINT I*2"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 6 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Delete_Line()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 I=3",
            "20 I = I * 2",
            "30 PRINT I",
            "20"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Assignment_Command()
    {
        using var controller = new TestController();
        var statements = new List<string> {
            "a=3",
            "print a"
        };

        controller.ExecuteStatements(statements);

        controller.ReadOutputLine().Should().Be(" 3 ");
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
    public void Interpreter_Can_Clear_Variables_Without_Clearing_The_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A = 3",
            "20 CLEAR",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
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
}