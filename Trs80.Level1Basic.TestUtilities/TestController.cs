using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.TestUtilities;

public class TestController : DisposableBase
{
    private bool _disposed;
    private StringReader? _outputReader;
    private StringReader? _errorReader;
    public IScanner Scanner { get; }
    public IParser Parser { get; }
    private readonly IInterpreter _interpreter;
    private readonly StringWriter _output = new();
    private readonly StringWriter _error = new();

    private readonly Action? _onExplicitDispose;
    private readonly Action? _onImplicitDispose;

    public ITrs80 Trs80 { get; set; }

    public TextReader Input
    {
        get { return Trs80.In; }
        set { Trs80.In = value; }
    }

    public TestController(Action onExplicitDispose, Action onImplicitDispose) : this()    {
        _onExplicitDispose = onExplicitDispose;
        _onImplicitDispose = onImplicitDispose;
    }
    
    public TestController()
    {
        var bootstrapper = new Bootstrapper();
        IAppSettings? appSettings = bootstrapper.AppSettings;
        ILoggerFactory? loggerFactory = bootstrapper.LogFactory;

        INativeFunctions natives = new NativeFunctions();
        IHost host = new FakeHost();
        Trs80 = new VirtualMachine.Machine.Trs80(appSettings, loggerFactory, host)
        {
            Out = _output,
            Error = _error,
        };
        Scanner = new Scanner(Trs80, natives, appSettings);
        Parser = new Parser(Trs80, natives, appSettings);
        IProgram program = new BasicProgram(Scanner, Parser);
        IMachine environment = new Machine(Trs80, program);
        ITrs80Api trs80Api = new Trs80Api(program, Trs80);
        _interpreter = new Interpreter(host, Trs80, trs80Api, environment, program, appSettings);
    }

    protected override void DisposeExplicit() => _onExplicitDispose?.DynamicInvoke();
    protected override void DisposeImplicit() => _onImplicitDispose?.DynamicInvoke();

    public void ExecuteLine(string input)
    {
        var sourceLine = new SourceLine(input);
        List<Token> tokens = Scanner.ScanTokens(sourceLine);
        IStatement statement = Parser.Parse(tokens);
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

    private void InitializeErrorReader()
    {
        _errorReader = new StringReader(_error.ToString());
    }

    public string? ReadErrorLine()
    {
        if (_errorReader is null) InitializeErrorReader();

        return _errorReader!.ReadLine();
    }

    public bool IsEndOfRun()
    {
        if (_outputReader is null) InitializeOutputReader();

        string? firstLine = _outputReader!.ReadLine();

        return firstLine == "READY" || (firstLine == "" && _outputReader.ReadLine() == "READY");
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _outputReader?.Dispose();
            _output.Dispose();
        }
        base.Dispose(disposing);
        _disposed = true;
    }

    ~TestController()
    {
        Dispose(false);
    }
}