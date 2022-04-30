using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trs80.Level1Basic.Domain;
using Trs80.Level1Basic.Services.Parser;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter;

public class BasicEnvironment : IBasicEnvironment
{
    private readonly GlobalVariables _globals = new();

    public Stack<ForCheckCondition> ForChecks { get; } = new();
    public Stack<Statement> ProgramStack { get; } = new();
    private readonly IBuiltinFunctions _builtins;
    public DataElements Data { get; } = new();

    private readonly IConsole _console;
    private readonly IParser _parser;
    private readonly IScanner _scanner;

    public BasicEnvironment(IConsole console, IParser parser, IScanner scanner, IBuiltinFunctions builtins, IProgram program)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _builtins = builtins ?? throw new ArgumentNullException(nameof(builtins));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        Program = program ?? throw new ArgumentNullException(nameof(program));

        System.Console.CancelKeyPress += delegate (object _, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExecutionHalted = true;
        };
    }

    public void InitializeProgram()
    {
        Program.Initialize();
    }

    public IProgram Program { get; }

    public bool ExecutionHalted { get; private set; }

    public dynamic AssignVariable(string name, dynamic value)
    {
        return _globals.Assign(name, value);
    }

    public dynamic AssignArray(string name, int index, dynamic value)
    {
        return _globals.AssignArray(name, index, value);
    }

    public dynamic GetVariable(string name)
    {
        return _globals.Get(name);
    }

    public bool VariableExists(string name)
    {
        return _globals.Exists(name);
    }

    public FunctionDefinition GetFunctionDefinition(string name)
    {
        try
        {
            return _builtins.Get(name);
        }
        catch
        {
            return null;
        }
    }

    public void ListProgram(int lineNumber)
    {
        foreach (var line in Program.List().Where(s => s.LineNumber >= lineNumber))
            _console.WriteLine(line.LineNumber > 0 ? $" {line.LineNumber}  {line.SourceLine}" : $"{line.SourceLine}");
    }

    public void SaveProgram(string path)
    {
        var oldWriter = _console.Out;
        using var newWriter = new StreamWriter(path);
        _console.Out = newWriter;

        ListProgram(0);

        _console.Out = oldWriter;
    }

    public void LoadProgram(string path)
    {
        using var reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            List<Token> tokens = _scanner.ScanTokens(reader.ReadLine());
            var line = _parser.Parse(tokens);
            Program.AddLine(line);
        }
    }

    public void NewProgram()
    {
        Program.Clear();
        Initialize();
    }

    private Statement _nextStatement;
    public void RunProgram(Statement statement, IBasicInterpreter interpreter)
    {
        ExecutionHalted = false;

        while (statement != null && !ExecutionHalted)
        {
            _nextStatement = statement.Next;
            interpreter.Execute(statement);
            statement = _nextStatement;
        }
    }
    
    public int MemoryInUse()
    {
        return Program.Size();
    }

    public Statement GetStatementByLineNumber(int lineNumber)
    {
        return Program.GetExecutableStatement(lineNumber);
    }
    
    public void SetNextStatement(Statement statement)
    {
        _nextStatement = statement;
    }

    public void HaltRun()
    {
        ExecutionHalted = true;
    }

    public void LoadData(IBasicInterpreter interpreter)
    {
        Data.Clear();

        foreach (var dataStatement in Program.List().SelectMany(s => s.Statements).Where(s => s is Data))
            interpreter.Execute(dataStatement);
    }

    public Statement GetNextStatement()
    {
        return _nextStatement;
    }

    public void Initialize()
    {
        _globals.Clear();
        Data.MoveFirst();
    }

    public dynamic GetArrayValue(string name, int index)
    {
        return _globals.GetArrayValue(name, index);
    }
}