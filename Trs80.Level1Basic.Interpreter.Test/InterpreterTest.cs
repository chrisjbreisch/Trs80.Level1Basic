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

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class InterpreterTest
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

        _console = new Console.Console(appSettings, loggerFactory, false);
        _console.Out = _sw;

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
        foreach (string line in program)
            ExecuteLine(line);

        ExecuteLine("run");
    }

    [TestMethod]
    public void Interpreter_Can_Run_HelloWorld()
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
    public void Interpreter_Can_Execute_Simple_Assignment()
    {
        var program = new List<string> {
            "10 i=3",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multiple_Assignments()
    {
        var program = new List<string> {
            "10 i=3",
            "15 i=7",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 7 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Goto()
    {
        var program = new List<string> {
            "10 i=3",
            "20 goto 40",
            "30 i=7",
            "40 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_End()
    {
        var program = new List<string> {
            "10 end",
            "20 print \"How did I get here?\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("");
        output = sr.ReadLine();
        output.Should().Be("READY");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Gosub()
    {
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = 7",
            "110 return"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 7 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multi_Statement_Lines()
    {
        var program = new List<string> {
            "10 i=3 : print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Addition()
    {
        var program = new List<string> {
            "10 i=3",
            "20 i= i + 1",
            "30 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 4 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Correct_Statement_After_Gosub()
    {
        var program = new List<string> {
            "10 i=3 : gosub 100 : i = i + 1",
            "20 print i",
            "30 end",
            "100 i = 5",
            "110 return"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 6 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Process_String_Variables()
    {
        var program = new List<string> {
            "10 A$=\"Chris\"",
            "20 print \"Hello, \";A$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Hello, Chris");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Read_From_Mocked_Console()
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
}