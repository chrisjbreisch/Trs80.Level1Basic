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
public class PrintTest
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
    public void Cls_Resets_CursorX_And_CursorY()
    {
        ExecuteLine("print \"hello\"");
        _console!.CursorY.Should().NotBe(0);
        _console!.CursorX.Should().Be(0);

        ExecuteLine("cls");
        _console!.CursorX.Should().Be(0);
        _console!.CursorY.Should().Be(0);
    }

    [TestMethod]
    public void Interpreter_Can_Print_String()
    {
        var program = new List<string> {
            "10 print \"Hello, World!\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Hello, World!");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Positive_Integer()
    {
        var program = new List<string> {
            "10 i = 5",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Negative_Integer()
    {
        var program = new List<string> {
            "10 i = -5",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("-5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Large_Value()
    {
        var program = new List<string> {
            "10 i = 1000000",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be( " 1E+06 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Strings_Together()
    {
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 print a$;b$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("startup");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Strings_With_Comma()
    {
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 print a$,b$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("start           up");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Three_Strings_With_Comma()
    {
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 C$=\"shutdown\"",
            "40 print a$,b$,c$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("start           up              shutdown");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Four_Strings_With_Comma()
    {
        var program = new List<string> {
            "10 A$=\"start\"",
            "20 B$=\"up\"",
            "30 C$=\"shut\"",
            "40 D$=\"down\"",
            "50 print a$,b$,c$,d$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("start           up              shut            down");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Integers_Together()
    {
        var program = new List<string> {
            "10 A = 3",
            "20 B = 5",
            "30 print a;b"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3  5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Integers_With_Comma()
    {
        var program = new List<string> {
            "10 A = 3",
            "20 B = 5",
            "30 print a,b"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3               5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_Two_Integers_On_Separate_Statements()
    {
        var program = new List<string> {
            "10 A = 3",
            "20 B = 5",
            "30 print a;",
            "40 print b"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3  5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_At_A_Position()
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
    public void Interpreter_Can_Print_With_Tab()
    {
        var program = new List<string> {
            "10 print tab(5); \"hello\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("     hello");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_With_Tabs()
    {
        var program = new List<string> {
            "10 print tab(5); \"hello\";tab(8);\"goodbye\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("     hellogoodbye");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Print_With_Many_Tabs()
    {
        var program = new List<string> {
            "10 a=1:b=2:c=3:d=4:e=5:f=6:g=7:h=8:i=9:j=10",
            "20 print a;tab(5);b;tab(10);c;tab(15);d;tab(20);",
            "30 print e;tab(25);f;tab(30);g;tab(35);h;tab(40);",
            "40 print i;tab(45);j"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1    2    3    4    5    6    7    8    9    10 ");
        sr.ReadToEnd();
    }
}

