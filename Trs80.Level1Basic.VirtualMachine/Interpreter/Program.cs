using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Program : IProgram
{
    private readonly LineList _statements = new();
    private readonly IScanner _scanner;
    private readonly IParser _parser;

    public IStatement CurrentStatement { get; set; }

    public Program(IScanner scanner, IParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public void Initialize()
    {
    }

    public IStatement GetExecutableStatement(int lineNumber)
    {
        IStatement statement = _statements
            .FirstOrDefault(s => s.LineNumber == lineNumber && ((IListLineDecorator)s).BaseType() != typeof(Data));

        if (statement is not null) return statement;
        statement = _statements
            .FirstOrDefault(s => s.LineNumber == lineNumber && ((IListLineDecorator)s).BaseType() == typeof(Data));

        if (statement is null) return null;

        return _statements
            .FirstOrDefault(s => s.LineNumber >= lineNumber && ((IListLineDecorator)s).BaseType() != typeof(Data));
    }

    public LineList List()
    {
        return _statements;
    }

    public void Clear()
    {
        _statements.Clear();
    }

    public void Load(string path)
    {
        using var reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            List<Token> tokens = _scanner.ScanTokens(reader.ReadLine());
            IStatement statement = _parser.Parse(tokens);
            AddStatement(statement);
        }
    }

    public void RemoveStatement(IStatement statement)
    {
        _statements.Remove(statement);
    }

    public int Size()
    {
        return _statements.Sum(statement => 4 + statement.SourceLine.Length);
    }

    public void ReplaceStatement(IStatement statement)
    {
        _statements.Replace(statement.LineNumber, statement);
    }

    private void AddStatement(IStatement statement)
    {
        if (statement == null) return;

        if (_statements.ContainsLine(statement.LineNumber))
            _statements.Replace(statement.LineNumber, statement);
        else
            _statements.Add(statement);
    }

    public IStatement GetFirstStatement()
    {
        return _statements.FirstOrDefault();
    }
}