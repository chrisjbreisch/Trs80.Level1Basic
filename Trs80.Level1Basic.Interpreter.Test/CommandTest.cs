using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Command.Commands;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

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
    public void Input_Command_Exits_Auto_On_Pause_Break()
    {
        using var controller = new TestController();
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        IProgram program = new BasicProgram(controller.Scanner, controller.Parser);
        var command = new InputCommand(controller.Trs80, history, auto, program, controller.PendingEdit);
        var model = new InputModel { WritePrompt = true };

        controller.Host.EnqueueKey(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('U', ConsoleKey.U, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('T', ConsoleKey.T, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('O', ConsoleKey.O, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.Pause, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        command.Execute(model);

        auto.IsActive.Should().BeFalse();
        model.SourceLine.Line.Should().BeEmpty();
    }

    [TestMethod]
    public void Input_Command_Auto_Advances_Numbered_Source_Lines()
    {
        using var controller = new TestController();
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        IProgram program = new BasicProgram(controller.Scanner, controller.Parser);
        var command = new InputCommand(controller.Trs80, history, auto, program, controller.PendingEdit);
        var firstModel = new InputModel();
        var secondModel = new InputModel();
        auto.Start();

        controller.Host.EnqueueKey(new ConsoleKeyInfo('P', ConsoleKey.P, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('R', ConsoleKey.R, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('I', ConsoleKey.I, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('N', ConsoleKey.N, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('T', ConsoleKey.T, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        command.Execute(firstModel);

        controller.Host.EnqueueKey(new ConsoleKeyInfo('P', ConsoleKey.P, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('R', ConsoleKey.R, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('I', ConsoleKey.I, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('N', ConsoleKey.N, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('T', ConsoleKey.T, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        command.Execute(secondModel);

        firstModel.SourceLine.Line.Should().Be("10 PRINT 1");
        secondModel.SourceLine.Line.Should().Be("20 PRINT 2");
        auto.IsActive.Should().BeTrue();
    }

    [TestMethod]
    public void Input_Command_AU_Abbreviation_Starts_Auto_Numbering()
    {
        using var controller = new TestController();
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        IProgram program = new BasicProgram(controller.Scanner, controller.Parser);
        var command = new InputCommand(controller.Trs80, history, auto, program, controller.PendingEdit);
        var model = new InputModel();

        QueueInput(controller, "AU.");
        QueueInput(controller, "PRINT 1");
        command.Execute(model);

        model.SourceLine.Line.Should().Be("10 PRINT 1");
        auto.IsActive.Should().BeTrue();
    }

    [TestMethod]
    public void Input_Command_Opens_Edit_Mode_After_Numbered_Syntax_Error()
    {
        using var controller = new TestController();
        controller.ExecuteLine("60 P=M*(1+I");
        controller.ExecuteLine("RUN");
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        var command = new InputCommand(controller.Trs80, history, auto, controller.Program, controller.PendingEdit);
        var model = new InputModel { WritePrompt = true };

        controller.Host.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo(')', ConsoleKey.D0, true, false, false));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        command.Execute(model);

        model.SourceLine.Line.Should().Be("60 P=M*(1+I)");
    }

    [TestMethod]
    public void Input_Command_Edit_Blank_Line_Deletes_The_Selected_Line()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        var command = new InputCommand(controller.Trs80, history, auto, controller.Program, controller.PendingEdit);
        var model = new InputModel();

        QueueInput(controller, "EDIT 10");
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.U, false, false, true));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        command.Execute(model);

        model.SourceLine.Line.Should().Be("10 ");
        controller.ExecuteLine(model.SourceLine.Original);
        controller.Program.List().Should().ContainSingle(statement => statement.LineNumber == 20);
    }

    [TestMethod]
    public void Input_Command_Pending_Edit_Blank_Line_Deletes_The_Selected_Line()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        var command = new InputCommand(controller.Trs80, history, auto, controller.Program, controller.PendingEdit);
        var model = new InputModel();

        controller.PendingEdit.Request(10);
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.U, false, false, true));
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        command.Execute(model);

        model.SourceLine.Line.Should().Be("10 ");
        controller.ExecuteLine(model.SourceLine.Original);
        controller.Program.List().Should().ContainSingle(statement => statement.LineNumber == 20);
    }

    [TestMethod]
    public void Input_Command_ED_Abbreviation_Selects_Line_For_Editing()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });
        var history = new LineEditorHistory();
        var auto = new AutoLineNumbering();
        var command = new InputCommand(controller.Trs80, history, auto, controller.Program, controller.PendingEdit);
        var model = new InputModel();

        QueueInput(controller, "ED. 10");
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.U, false, false, true));
        QueueInput(controller, "PRINT 11");
        command.Execute(model);

        model.SourceLine.Line.Should().Be("10 PRINT 11");
    }

    private static void QueueInput(TestController controller, string text)
    {
        foreach (char character in text)
            controller.Host.EnqueueKey(new ConsoleKeyInfo(character, ConsoleKey.A, false, false, false));

        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
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
    public void Parser_Can_Parse_Abbreviated_Input_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("IN. A"));

        controller.Parser.Parse(tokens).Should().BeOfType<Input>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Abbreviated_Read_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("REA. A"));

        controller.Parser.Parse(tokens).Should().BeOfType<Read>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Abbreviated_Return_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("RET."));

        controller.Parser.Parse(tokens).Should().BeOfType<Return>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Cassette_Command_Aliases_As_File_Commands()
    {
        using var controller = new TestController();

        List<Token> loadTokens = controller.Scanner.ScanTokens(new SourceLine("CLOAD \"PROGRAM.BAS\""));
        List<Token> saveTokens = controller.Scanner.ScanTokens(new SourceLine("CSAVE \"PROGRAM.BAS\""));

        controller.Parser.Parse(loadTokens).Should().BeOfType<Load>();
        controller.Parser.Parse(saveTokens).Should().BeOfType<Save>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Out_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("OUT 255, 1"));

        controller.Parser.Parse(tokens).Should().BeOfType<Out>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Wait_Command_With_Optional_Invert_Mask()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("WAIT 255, 1, 2"));

        var statement = controller.Parser.Parse(tokens).Should().BeOfType<Wait>().Subject;

        statement.Invert.Should().NotBeNull();
    }

    [TestMethod]
    public void Parser_Can_Parse_Lprint_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("LPRINT \"HELLO\""));

        controller.Parser.Parse(tokens).Should().BeOfType<Lprint>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Llist_Command_With_Range()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("LLIST 10-20"));

        var statement = controller.Parser.Parse(tokens).Should().BeOfType<Llist>().Subject;

        statement.EndAtLineNumber.Should().NotBeNull();
    }

    [TestMethod]
    public void Parser_Can_Parse_System_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("SYSTEM"));

        controller.Parser.Parse(tokens).Should().BeOfType<SystemStatement>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Reset_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("RESET 23, 20"));

        controller.Parser.Parse(tokens).Should().BeOfType<ResetStatement>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Set_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("SET 23, 20"));

        controller.Parser.Parse(tokens).Should().BeOfType<SetStatement>();
    }

    [TestMethod]
    public void Parser_Can_Parse_Poke_Command()
    {
        using var controller = new TestController();
        var tokens = controller.Scanner.ScanTokens(new SourceLine("POKE 100, 65"));

        controller.Parser.Parse(tokens).Should().BeOfType<PokeStatement>();
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
    public void Interpreter_Can_List_Only_A_Single_Line()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20",
            "30 PRINT 30"
        });

        controller.ExecuteLine("LIST 20");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
    }

    [TestMethod]
    public void Interpreter_Can_List_The_Last_Entered_Line()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });

        controller.ExecuteLine("LIST .");

        controller.ReadOutputLine().Should().Be(" 20  PRINT 20");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
    }

    [TestMethod]
    public void Interpreter_Can_List_The_Last_Edited_Line()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 10",
            "20 PRINT 20"
        });

        controller.ExecuteLine("10 PRINT UPDATED");
        controller.ExecuteLine("LIST .");

        controller.ReadOutputLine().Should().Be(" 10  PRINT UPDATED");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
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
    public void Interpreter_Does_Not_Pause_After_The_Final_List_Page()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 1", "20 PRINT 2", "30 PRINT 3", "40 PRINT 4",
            "50 PRINT 5", "60 PRINT 6", "70 PRINT 7", "80 PRINT 8",
            "90 PRINT 9", "100 PRINT 10", "110 PRINT 11", "120 PRINT 12"
        });
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        controller.ExecuteLine("LIST");

        controller.Host.ReadKeyCount.Should().Be(0);
        for (int line = 1; line <= 12; line++)
            controller.ReadOutputLine().Should().Be($" {line * 10}  PRINT {line}");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
    }

    [TestMethod]
    public void Interpreter_Pauses_When_Another_List_Page_Remains()
    {
        using var controller = new TestController();
        controller.ExecuteStatements(new List<string> {
            "10 PRINT 1", "20 PRINT 2", "30 PRINT 3", "40 PRINT 4",
            "50 PRINT 5", "60 PRINT 6", "70 PRINT 7", "80 PRINT 8",
            "90 PRINT 9", "100 PRINT 10", "110 PRINT 11", "120 PRINT 12",
            "130 PRINT 13"
        });
        controller.Host.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

        controller.ExecuteLine("LIST");

        controller.Host.ReadKeyCount.Should().Be(1);
        for (int line = 1; line <= 13; line++)
            controller.ReadOutputLine().Should().Be($" {line * 10}  PRINT {line}");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
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

        controller.ReadOutputLine().Should().Be(" 15572 ");
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

        controller.ReadOutputLine().Should().Be(" 15562 ");
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

        controller.ReadOutputLine().Should().Be(" 15510 ");
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