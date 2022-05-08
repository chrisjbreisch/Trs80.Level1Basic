using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.Interpreter.Interpreter;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;
using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class LonghandSmokeTest
{
    private IScanner? _scanner;
    private IParser? _parser;
    private IBasicInterpreter? _interpreter;
    private IBasicEnvironment? _environment;
    private IConsole? _console;
    private readonly StringWriter _sw = new();

    [TestInitialize]
    public void Initialize()
    {
        var bootstrapper = new Bootstrapper();
        IAppSettings? appSettings = bootstrapper.AppSettings;
        ILoggerFactory? loggerFactory = bootstrapper.LogFactory;

        _console = new Console.Console(appSettings, loggerFactory, new FakeSystemConsole())
        {
            Out = _sw
        };

        IBuiltinFunctions builtins = new BuiltinFunctions();
        _scanner = new Scanner.Scanner(builtins);
        _parser = new Parser.Parser(builtins);
        IProgram program = new Program();
        _environment = new BasicEnvironment(_console, _parser, _scanner, builtins, program);
        _interpreter = new BasicInterpreter(_console, _environment);
    }

    private void ExecuteLine(string input)
    {
        List<Token> tokens = _scanner!.ScanTokens(input);
        ParsedLine parsedLine = _parser!.Parse(tokens);
        _interpreter!.Interpret(parsedLine);
    }

    private void RunProgram(List<string> program)
    {
        ExecuteLine("new");

        ExecuteStatements(program);

        ExecuteLine("run");
    }

    private void ExecuteStatements(List<string> statements)
    {
        foreach (string line in statements)
            ExecuteLine(line);
    }

    [TestMethod]
    public void Interpreter_Executes_Print()
    {
        var program = new List<string> {
            "10 print \"hello\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("hello");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void New_Clears_Out_Program()
    {
        var program = new List<string> {
            "10 print \"hello\"",
        };
        ExecuteStatements(program);
        ExecuteLine("new");
        ExecuteLine("print mem");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15871 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Program()
    {
        var program = new List<string> {
            "10 print \"hello\"",
        };
        ExecuteStatements(program);
        ExecuteLine("run");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("hello");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpret_Can_List_Program()
    {
        var program = new List<string> {
            "10 print \"hello\"",
        };
        ExecuteStatements(program);
        ExecuteLine("list");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 10  print \"hello\"");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void End_Terminates_Program_Execution()
    {
        var program = new List<string> {
            "10 i=3",
            "20 end",
            "30 print i"
        };
        RunProgram(program);
        
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Then()
    {
        var program = new List<string> {
            "10 i=3",
            "20 if i=3 then print i:end",
            "30 print i+1"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Goto_Transfers_Execution()
    {
        var program = new List<string> {
            "10 i=3",
            "20 goto 100",
            "30 print i",
            "40 end",
            "100 print i+1"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 4 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Input_Reads_From_User()
    {
        using var input = new StringReader("Chris");
        _console!.In = input;

        var program = new List<string> {
            "10 input \"Enter your name\";A$",
            "20 print \"Hello, \";A$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Enter your name?Hello, Chris");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Print_Mem_Displays_Free_Memory_Size()
    {
        ExecuteLine("new");
        ExecuteLine("print mem");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15871 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void For_Loops_Execute_Properly()
    {
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10",
            "30 i=i*2",
            "40 next n",
            "50 print i"
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1024 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Step_Controls_Increment_In_For_Loop()
    {
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10 step 2",
            "30 i=i*2",
            "40 next n",
            "50 print i"
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 32 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Step_Can_Be_Negative()
    {
        var program = new List<string> {
            "10 for n=10 to 1 step -1",
            "20 print n;",
            "30 next n",
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 10  9  8  7  6  5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Stop_Halts_Execution()
    {
        var program = new List<string> {
            "10 i=1",
            "20 stop",
            "30 print i",
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("BREAK AT 20");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Cont_Resumes_After_Stop()
    {
        var program = new List<string> {
            "10 i=3",
            "20 stop",
            "30 print i",
        };

        RunProgram(program);
        ExecuteLine("cont");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("BREAK AT 20");

        output = sr.ReadLine();
        output.Should().Be("");

        output = sr.ReadLine();
        output.Should().Be("READY");

        output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Gosub_Calls_Subroutine_And_Returns()
    {
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = i * 2",
            "110 return"
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 6 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Read_Gets_Values_From_Data_Statement()
    {
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 print e;d;c;b;a",
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Restore_Resets_Data_Stream_To_Original()
    {
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 restore",
            "40 read f,g,h,i,j",
            "50 print j;i;h;g;f"
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Print_At_Prints_To_A_Position_On_Screen()
    {
        var program = new List<string> {
            "10 print at 200, \"hello\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("hello");
        _console!.CursorY.Should().Be(200 / 64 + 3);
        _console.CursorX.Should().Be(0);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Handle_Multi_Statement_Lines()
    {
        var program = new List<string> {
            "10 i = 3 : if i = 3 gosub 100",
            "20 for n = 1 to 10 : i = i * 2 : next n : goto 200",
            "30 print \"BAD\" : end",
            "100 i = 1: return",
            "110 print \"WRONG\" : end",
            "200 print i : end",
            "210 print \"FAIL\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1024 ");

        output = sr.ReadLine();
        output.Should().Be("");

        output = sr.ReadLine();
        output.Should().Be("READY");
        sr.ReadToEnd();
    }
}