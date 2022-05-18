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
    //private List<ParsedLine> _programLines = new();
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
    }

    private void AddStatement(Statement statement)
    {
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

    public LinkedList<Statement> List()
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
            ParsedLine line = _parser.Parse(tokens);
            AddLine(line);
        }
    }

    public void RemoveLine(Statement line)
    {
        Statement existing = _statements.FirstOrDefault(s => s.LineNumber == line.LineNumber);
        if (existing == null) return;

        _statements.Remove(existing);
    }

    public int Size()
    {
        return _statements.Sum(statement => 4 + statement.SourceLine.Length);
    }

    private void AddLine(ParsedLine line)
    {
        if (line == null) return;
        Statement programLine = GetProgramLine(line);

        if (programLine != null)
            ReplaceLine(line);
        else
            AddStatement(line.Statement);
    }

    private Statement GetProgramLine(ParsedLine line)
    {
        Statement programLine = _statements.FirstOrDefault(l => l.LineNumber == line.LineNumber);
        return programLine;
    }

    public void ReplaceLine(ParsedLine line)
    {
        if (line == null) return;
        Statement statement = _statements.FirstOrDefault(s => s.LineNumber == line.LineNumber);
        if (statement == null)
            AddLine(line);
        else
        {
            if (_statements.Count == 1)
            {
                _statements.RemoveFirst();
                _statements.AddFirst(line.Statement);
            }
            else
            {
                LinkedListNode<Statement> originalNode = _statements.Find(statement);
                LinkedListNode<Statement> successor = originalNode.Next;
                if (successor != null)
                    _statements.AddBefore(successor!, line.Statement);
                else
                {
                    LinkedListNode<Statement> predecessor = originalNode.Previous;
                    if (predecessor != null) _statements.AddAfter(predecessor, line.Statement);
                }

                _statements.Remove(statement);
            }
        }
    }

    public Statement GetFirstStatement()
    {
        return _statements.FirstOrDefault();
    }

    public Statement CurrentStatement { get; set; }
}