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
    private List<ParsedLine> _programLines = new();
    private readonly List<Statement> _programStatements = new();
    private bool _sorted;
    private readonly IScanner _scanner;
    private readonly IParser _parser;

    public Program(IScanner scanner, IParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public void Initialize()
    {
        _programStatements.Clear();
        _sorted = false;
        Sort();

        Statement last = null;
        foreach (Statement statement in _programLines.SelectMany(line => line.Statements))
        {
            if (last != null)
                last.Next = statement;
            _programStatements.Add(statement);
            last = statement;
        }
    }

    public Statement GetExecutableStatement(int lineNumber)
    {
        Statement statement = _programStatements
            .FirstOrDefault(s => s.LineNumber == lineNumber && s is not Data);

        if (statement is not null) return statement;
        statement = _programStatements
            .FirstOrDefault(s => s.LineNumber == lineNumber && s is Data);

        if (statement is null) return null;

        return _programStatements
            .FirstOrDefault(s => s.LineNumber >= lineNumber && s is not Data);
    }

    public List<ParsedLine> List()
    {
        Sort();
        return _programLines;
    }

    public void Clear()
    {
        _programLines.Clear();
    }

    public void Load(string path)
    {
        using var reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            List<Token> tokens = _scanner.ScanTokens(reader.ReadLine());
            ParsedLine line = _parser.Parse(tokens);
            AddLine(line);
        }
    }

    public void RemoveLine(ParsedLine line)
    {
        IEnumerable<Statement> previousLines = _programLines.SelectMany(s => s.Statements).Where(p => p?.Next?.LineNumber == line.LineNumber);

        foreach (Statement previousLine in previousLines)
            previousLine.Next = line.Statements[0].Next;

        _programLines.Remove(line);
    }

    public int Size()
    {
        return _programLines.Sum(statement => 4 + statement.SourceLine.Length);
    }

    private void AddLine(ParsedLine line)
    {
        if (line == null) return;
        ParsedLine programLine = GetProgramLine(line);

        if (programLine != null)
            ReplaceLine(line);
        else
            _programLines.Add(line);

        _sorted = false;
    }

    private ParsedLine GetProgramLine(ParsedLine line)
    {
        ParsedLine programLine = _programLines.FirstOrDefault(l => l.LineNumber == line.LineNumber);
        return programLine;
    }

    public void ReplaceLine(ParsedLine line)
    {
        if (line == null) return;
        ParsedLine programLine = GetProgramLine(line);

        if (programLine != null)
        {
            programLine.SourceLine = line.SourceLine;
            programLine.Statements = line.Statements;
        }
        else
        {
            AddLine(line);
            Sort();
        }
    }

    public Statement GetFirstStatement()
    {
        Sort();
        return _programStatements.FirstOrDefault();
    }

    public void Sort()
    {
        if (_sorted) return;

        _programLines = _programLines.OrderBy(l => l.LineNumber).ToList();
        _sorted = true;
    }
}