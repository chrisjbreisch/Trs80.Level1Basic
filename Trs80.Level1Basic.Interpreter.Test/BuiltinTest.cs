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
public class BuiltinTest
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
    public void Interpreter_Can_Call_Abs()
    {
        var program = new List<string> {
            "10 a=-3.14:b=abs(a):c=a.(b)",
            "20 print a;b;c"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("-3.14  3.14  3.14 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Chr()
    {
        var program = new List<string> {
            "10 print chr$(34);\"hello\";chr$(34)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("\"hello\"");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Int()
    {
        var program = new List<string> {
            "10 a=-3.14:b=int(a):c=3.14:d=i.(c)",
            "20 print a;b;c;d"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("-3.14 -4  3.14  3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Mem()
    {
        var program = new List<string> {
            "10 print mem;m.",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15855  15855 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Set_And_Point()
    {
        var program = new List<string> {
            "10 set(23,20):s.(46,40)",
            "20 if point(23,20) * p.(46,40) then print \"ON\": end",
            "30 print \"OFF\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("ON");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Reset_And_Point()
    {
        var program = new List<string> {
            "10 set(23,20):s.(46,40)",
            "15 reset(23,20):r.(46,40)",
            "20 if point(23,20) + p.(46,40) then print \"ON\": end",
            "30 print \"OFF\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("OFF");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_0()
    {
        var program = new List<string> {
            "10 print rnd(0)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_2()
    {
        var program = new List<string> {
            "10 print rnd(2)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        int value = int.Parse(output!);
        value.Should().BeInRange(1, 2);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_10()
    {
        var program = new List<string> {
            "10 print rnd(10)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        int value = int.Parse(output!);
        value.Should().BeInRange(1, 10);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_Rnd_100()
    {
        var program = new List<string> {
            "10 print rnd(100)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        int value = int.Parse(output!);
        value.Should().BeInRange(1, 100);
        sr.ReadToEnd();
    }
    [TestMethod]
    public void Interpreter_Can_Call_R_0()
    {
        var program = new List<string> {
            "10 print r.(0)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        float value = float.Parse(output!);
        value.Should().BeInRange(0, 1);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_2()
    {
        var program = new List<string> {
            "10 print r.(2)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        int value = int.Parse(output!);
        value.Should().BeInRange(1, 2);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_10()
    {
        var program = new List<string> {
            "10 print r.(10)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        int value = int.Parse(output!);
        value.Should().BeInRange(1, 10);
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Call_R_100()
    {
        var program = new List<string> {
            "10 print r.(100)",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        int value = int.Parse(output!);
        value.Should().BeInRange(1, 100);
        sr.ReadToEnd();
    }
}