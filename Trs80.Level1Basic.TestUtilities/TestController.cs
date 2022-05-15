using Microsoft.Extensions.Logging;
using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.VirtualMachine.Environment;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Environment = Trs80.Level1Basic.VirtualMachine.Environment.Environment;

namespace Trs80.Level1Basic.TestUtilities;

public class TestController : IDisposable
{
    private bool _disposed;
    private StringReader? _outputReader;
    private readonly IScanner _scanner;
    private readonly IParser _parser;
    private readonly IInterpreter _interpreter;
    private readonly StringWriter _output = new();

    public IConsole Console { get; set; }

    public TextReader Input
    {
        get { return Console.In; }
        set { Console.In = value; }
    }

    public TestController()
    {
        var bootstrapper = new Bootstrapper();
        IAppSettings? appSettings = bootstrapper.AppSettings;
        ILoggerFactory? loggerFactory = bootstrapper.LogFactory;

        Console = new Console.Console(appSettings, loggerFactory, new FakeSystemConsole())
        {
            Out = _output
        };

        IBuiltinFunctions builtins = new BuiltinFunctions();
        _scanner = new Scanner(builtins);
        _parser = new Parser(builtins);
        IProgram program = new Program();
        IEnvironment environment = new Environment(Console, _parser, _scanner, builtins, program);
        _interpreter = new Interpreter(Console, environment);
    }

    public void ExecuteLine(string input)
    {
        List<Token> tokens = _scanner.ScanTokens(input);
        ParsedLine parsedLine = _parser.Parse(tokens);
        _interpreter.Interpret(parsedLine);
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