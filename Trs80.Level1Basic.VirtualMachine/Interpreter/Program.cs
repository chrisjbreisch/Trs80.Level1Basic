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
    //private readonly List<Statement> _programStatements = new();
    private readonly LinkedList<Statement> _statements = new();
    private readonly IScanner _scanner;
    private readonly IParser _parser;

    public Program(IScanner scanner, IParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public void Initialize()
    {

        _statements.Clear();

        Statement last = null;
        foreach (Statement statement in _programLines.Select(line => line.Statement))
            if (statement is Compound compound)
                last = compound.Statements.Aggregate(last, AddStatementToList);
            else
                last = AddStatementToList(last, statement);
    }

    private Statement AddStatementToList(Statement last, Statement statement)
    {
        if (last != null)
            last.Next = statement;

        if (_statements.Count == 0)
            _statements.AddFirst(statement);
        else
        {
            Statement successor = _statements.FirstOrDefault(s => s.LineNumber > statement.LineNumber);
            if (successor != null)
            {
                LinkedListNode<Statement> node = _statements.Find(successor);
                _statements.AddBefore(node!, statement);
            }
            else
            {
                Statement predecessor = _statements.LastOrDefault(s => s.LineNumber < statement.LineNumber);
                if (predecessor != null)
                {
                    LinkedListNode<Statement> node = _statements.Find(predecessor);
                    _statements.AddAfter(node!, statement);
                }
            }
        }

        last = statement;
        return last;
    }

    public Statement GetExecutableStatement(int lineNumber)
    {
        Statement statement = _statements
            .FirstOrDefault(s => s.LineNumber == lineNumber && s is not Data);

        if (statement is not null) return statement;
        statement = _statements
            .FirstOrDefault(s => s.LineNumber == lineNumber && s is Data);

        if (statement is null) return null;

        return _statements
            .FirstOrDefault(s => s.LineNumber >= lineNumber && s is not Data);
    }

    public List<ParsedLine> List()
    {
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
        IEnumerable<Statement> previousLines = _programLines.Select(s => s.Statement).Where(p => p?.Next?.LineNumber == line.LineNumber);

        foreach (Statement previousLine in previousLines)
            previousLine.Next = line.Statement.Next;

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
            programLine.Statement = line.Statement;
        }
        else
        {
            AddLine(line);
        }
    }

    public Statement GetFirstStatement()
    {
        return _statements.FirstOrDefault();
    }

    public Statement CurrentStatement { get; set; }
}