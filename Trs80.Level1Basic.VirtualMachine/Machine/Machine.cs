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
    private IStatement _nextStatement;

    public int CursorX { get; set; }
    public int CursorY { get; set; }
    public DataElements Data { get; } = new();
    public IProgram Program { get; }
    public bool ExecutionHalted { get; set; }

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
    
    public dynamic Set(string name, dynamic value)
    {
        return _globals.Set(name, value);
    }

    public dynamic Set(string name, int index, dynamic value)
    {
        return _globals.AssignArray(name, index, value);
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
        foreach (IStatement statement in Program.List().Where(s => s.LineNumber >= lineNumber))
        {
            _trs80.WriteLine(statement.LineNumber >= 0 ? $" {statement.LineNumber}  {statement.SourceLine}" : $"{statement.SourceLine}");
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

        foreach (IStatement statement in Program.List())
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

    public void RunStatementList(IStatement statement, IInterpreter interpreter)
    {
        ExecutionHalted = false;
        if (statement == null) return;

        while (statement != null && !ExecutionHalted)
        {
            _nextStatement = GetNextStatement(statement);
            interpreter.Execute(statement);
            statement = _nextStatement;
        }
    }

    public void RunCompoundStatement(CompoundStatementList compound, IInterpreter interpreter)
    {
        IStatement nextStatement = _nextStatement;

        ExecutionHalted = false;
        if (compound == null) return;
        IStatement statement = compound[0];
        int lineNumber = statement.LineNumber;

        while (statement != null && !ExecutionHalted)
        {
            _nextStatement = statement.Next;
            interpreter.Execute(statement);
            if (statement is IListStatementDecorator decorated && decorated.BaseType() == typeof(Goto)) return;
            if (_nextStatement != null && _nextStatement.LineNumber != lineNumber) return;
            statement = _nextStatement;
        }
        SetNextStatement(nextStatement);
    }

    public IStatement GetStatementByLineNumber(int lineNumber)
    {
        return Program.GetExecutableStatement(lineNumber);
    }

    public void SetNextStatement(IStatement statement)
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

        foreach (IStatement dataStatement in Program.List().Where(s => s is Data))
            interpreter.Execute(dataStatement);
    }


    public IStatement GetNextStatement(IStatement statement)
    {
        if (statement == null) return null;
        if (statement.Next != null) return statement.Next;
        return statement is not IListStatementDecorator statementDecorator ? null : statementDecorator.Enclosing?.Next;
    }

    public IStatement GetNextStatement()
    {
        return _nextStatement;
    }

    public void Initialize()
    {
        _globals.InitializeVariables();
        Data.MoveFirst();
    }

    public dynamic Get(string name)
    {
        return _globals.Get(name);
    }

    public dynamic Get(string name, int index)
    {
        return _globals.GetArrayValue(name, index);
    }
}