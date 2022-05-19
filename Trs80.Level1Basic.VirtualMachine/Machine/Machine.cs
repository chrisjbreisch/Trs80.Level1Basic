using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public class Machine : IMachine
{
    private readonly Interpreter.Environment _globals = new();
    private readonly ITrs80 _trs80;
    private readonly INativeFunctions _natives;

    public int CursorX { get; set; }
    public int CursorY { get; set; }
    public Stack<ForCondition> ForConditions { get; } = new();
    public Stack<Statement> ProgramStack { get; } = new();
    public DataElements Data { get; } = new();
    public IProgram Program { get; }
    public bool ExecutionHalted { get; private set; }

    public Machine(ITrs80 trs80, IProgram program, INativeFunctions natives)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        Program = program ?? throw new ArgumentNullException(nameof(program));
        _natives = natives ?? throw new ArgumentNullException(nameof(natives));

        Console.CancelKeyPress += delegate (object _, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExecutionHalted = true;
        };
    }

    public void InitializeProgram()
    {
        Program.Initialize();
        GetCursorPosition();
    }


    private void GetCursorPosition()
    {
        (int left, int top) = _trs80.GetCursorPosition();
        CursorX = left;
        CursorY = top;
    }


    public dynamic Assign(string name, dynamic value)
    {
        return _globals.Assign(name, value);
    }

    public dynamic Assign(string name, int index, dynamic value)
    {
        return _globals.AssignArray(name, index, value);
    }

    public dynamic Get(string name)
    {
        return _globals.Get(name);
    }

    public List<Callable> Function(string name)
    {
        return _natives.Get(name);
    }

    public bool Exists(string name)
    {
        return _globals.Exists(name);
    }
    
    public void ListProgram(int lineNumber)
    {
        int index = 0;
        bool exitList = false;
        foreach (Statement line in Program.List().Where(s => s.LineNumber >= lineNumber))
        {
            _trs80.WriteLine(line.LineNumber >= 0 ? $" {line.LineNumber}  {line.SourceLine}" : $"{line.SourceLine}");
            index++;
            if (index < 12) continue;

            bool readAnotherKey = true;
            while (readAnotherKey)
            {
                ConsoleKeyInfo key = _trs80.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    _trs80.WriteLine();
                    exitList = true;
                    break;
                }

                if (key.Key == ConsoleKey.UpArrow)
                    readAnotherKey = false;
            }

            if (exitList)
                break;
        }
    }

    public void SaveProgram(string path)
    {
        TextWriter oldWriter = _trs80.Out;
        using var newWriter = new StreamWriter(path);
        _trs80.Out = newWriter;

        foreach (Statement statement in Program.List())
            _trs80.WriteLine(statement.LineNumber >= 0 ? $" {statement.LineNumber}  {statement.SourceLine}" : $"{statement.SourceLine}");

        _trs80.Out = oldWriter;
    }

    public void LoadProgram(string path)
    {
        Program.Load(path);
    }

    public void NewProgram()
    {
        Program.Clear();
        Initialize();
    }

    private Statement _nextStatement;
    public void RunStatementList(Statement statement, IInterpreter interpreter)
    {
        ExecutionHalted = false;

        while (statement != null && !ExecutionHalted)
        {
            _nextStatement = GetNextStatement(statement);
            interpreter.Execute(statement);
            statement = _nextStatement;
        }
    }

    public Statement GetNextStatement(Statement statement)
    {
        while (true)
        {
            if (statement.Next != null) return statement.Next;
            if (statement.Parent != null)
            {
                statement = statement.Parent;
                continue;
            }

            LinkedListNode<Statement> node = Program.List().Find(statement);
            return node != null ? node!.Next?.Value : null;
        }
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

    public void LoadData(IInterpreter interpreter)
    {
        Data.Clear();

        foreach (Statement dataStatement in Program.List().Where(s => s is Data))
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

    public dynamic Get(string name, int index)
    {
        return _globals.GetArrayValue(name, index);
    }
}