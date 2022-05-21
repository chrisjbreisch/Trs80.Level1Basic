﻿using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

using Environment = Trs80.Level1Basic.VirtualMachine.Machine.Machine;

namespace Trs80.Level1Basic.TestUtilities;

public class TestController : IDisposable
{
    private bool _disposed;
    private StringReader? _outputReader;
    private readonly IScanner _scanner;
    private readonly IParser _parser;
    private readonly IInterpreter _interpreter;
    private readonly StringWriter _output = new();

    public ITrs80 Trs80 { get; set; }

    public TextReader Input
    {
        get { return Trs80.In; }
        set { Trs80.In = value; }
    }

    public TestController()
    {
        var bootstrapper = new Bootstrapper();
        IAppSettings? appSettings = bootstrapper.AppSettings;
        ILoggerFactory? loggerFactory = bootstrapper.LogFactory;

        INativeFunctions natives = new NativeFunctions();
        _scanner = new Scanner(natives);
        _parser = new Parser(natives);
        IProgram program = new Program(_scanner, _parser);
        IHost host = new FakeHost();
        Trs80 = new VirtualMachine.Machine.Trs80(program, appSettings, loggerFactory, host)
        {
            Out = _output
        };
        IMachine environment = new Environment(Trs80, program, natives);
        _interpreter = new Interpreter(host, Trs80, environment, program);
    }

    public void ExecuteLine(string input)
    {
        List<Token> tokens = _scanner.ScanTokens(input);
        IStatement statement = _parser.Parse(tokens);
        _interpreter.Interpret(statement);
    }

    public void ExecuteStatements(List<string> statements)
    {
        foreach (string line in statements)
            ExecuteLine(line);
    }

    public void RunProgram(List<string> program)
    {
        ExecuteLine("new");

        ExecuteStatements(program);

        ExecuteLine("run");
    }

    private void InitializeOutputReader()
    {
        _outputReader = new StringReader(_output.ToString());
    }

    public string? ReadOutputLine()
    {
        if (_outputReader is null) InitializeOutputReader();

        return _outputReader!.ReadLine();
    }

    public bool IsEndOfRun()
    {
        if (_outputReader is null) InitializeOutputReader();

        string? firstLine = _outputReader!.ReadLine();

        return firstLine == "READY" || (firstLine == "" && _outputReader.ReadLine() == "READY");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _outputReader?.Dispose();
            _output.Dispose();
        }
        _disposed = true;
    }

    ~TestController()
    {
        Dispose(false);
    }
}