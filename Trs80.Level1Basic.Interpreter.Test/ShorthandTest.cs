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
public class ShorthandTest
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
    public void P_Is_Shorthand_For_Print()
    {
        var program = new List<string> {
            "10 p.\"hello\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("hello");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void N_Is_Shorthand_For_New()
    {
        var program = new List<string> {
            "10 p.\"hello\"",
        };
        ExecuteStatements(program);
        ExecuteLine("n.");
        ExecuteLine("print mem");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15871 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void R_Is_Shorthand_For_Run()
    {
        var program = new List<string> {
            "10 print \"hello\"",
        };
        ExecuteStatements(program);
        ExecuteLine("r.");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("hello");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void L_Is_Shorthand_For_List()
    {
        var program = new List<string> {
            "10 print \"hello\"",
        };
        ExecuteStatements(program);
        ExecuteLine("l.");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 10  print \"hello\"");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void E_Is_Shorthand_For_End()
    {
        var program = new List<string> {
            "10 i=3",
            "20 e.",
            "30 print i"
        };
        RunProgram(program);
        
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void T_Is_Shorthand_For_Then()
    {
        var program = new List<string> {
            "10 i=3",
            "20 if i=3 t. print i:end",
            "30 print i+1"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void G_Is_Shorthand_For_Goto()
    {
        var program = new List<string> {
            "10 i=3",
            "20 g.100",
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
    public void In_Is_Shorthand_For_Input()
    {
        using var input = new StringReader("Chris");
        _console!.In = input;

        var program = new List<string> {
            "10 in. \"Enter your name\";A$",
            "20 print \"Hello, \";A$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Enter your name?Hello, Chris");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void M_Is_Shorthand_For_Mem()
    {
        ExecuteLine("new");
        ExecuteLine("p.m.");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15871 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void F_Is_Shorthand_For_For()
    {
        var program = new List<string> {
            "10 i=1",
            "20 f.n=1 to 10",
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
    public void N_Is_Shorthand_For_Next()
    {
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10",
            "30 i=i*2",
            "40 n. n",
            "50 print i"
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1024 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void S_Is_Shorthand_For_Step()
    {
        var program = new List<string> {
            "10 i=1",
            "20 for n=1 to 10 s. 2",
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
    public void St_Is_Shorthand_For_Stop()
    {
        var program = new List<string> {
            "10 i=1",
            "20 st.",
            "30 print i",
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("BREAK AT 20");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void C_Is_Shorthand_For_Cont()
    {
        var program = new List<string> {
            "10 i=3",
            "20 stop",
            "30 print i",
        };

        RunProgram(program);
        ExecuteLine("c.");

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
    public void Gos_Is_Shorthand_For_Gosub()
    {
        var program = new List<string> {
            "10 i=3",
            "20 gos.100",
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
    public void Ret_Is_Shorthand_For_Return()
    {
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = i * 2",
            "110 ret."
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 6 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Rea_Is_Shorthand_For_Read()
    {
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 rea. a,b,c,d,e",
            "30 print e;d;c;b;a",
        };

        RunProgram(program);
        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void D_Is_Shorthand_For_Data()
    {
        var program = new List<string> {
            "10 d. 1,2,3,4,5",
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
    public void Rest_Is_Shorthand_For_Restore()
    {
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 rest.",
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
    public void Pa_Is_Shorthand_For_Print_At()
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
    public void Interpreter_Can_Handle_Extreme_Shorthand()
    {
        var program = new List<string> {
            "10 i=3:ifi=3gos.100",
            "20 f.n=1to10:i=i*2:n.n:g.200",
            "30 p.\"BAD\"",
            "100 i=1:ret.",
            "110 p.\"WRONG\"",
            "200 p.i:e.",
            "210 p.\"FAIL\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1024 ");
        sr.ReadToEnd();
    }
}