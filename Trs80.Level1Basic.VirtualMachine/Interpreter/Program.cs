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
    private readonly LinkedList<Statement> _statements = new();
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

    private void InsertStatementIntoList(Statement statement)
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
                if (predecessor == null) return;

                LinkedListNode<Statement> node = _statements.Find(predecessor);
                _statements.AddAfter(node!, statement);
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
            Statement statement = _parser.Parse(tokens);
            AddStatement(statement);
        }
    }

    public void RemoveStatement(Statement statement)
    {
        Statement existing = _statements.FirstOrDefault(s => s.LineNumber == statement.LineNumber);
        if (existing == null) return;

        _statements.Remove(existing);
    }

    public int Size()
    {
        return _statements.Sum(statement => 4 + statement.SourceLine.Length);
    }

    private void AddStatement(Statement statement)
    {
        if (statement == null) return;

        if (FindMatchingStatement(statement) != null)
            ReplaceStatement(statement);
        else
            InsertStatementIntoList(statement);
    }

    private Statement FindMatchingStatement(Statement statement)
    {
        Statement match = _statements.FirstOrDefault(l => l.LineNumber == statement.LineNumber);
        return match;
    }

    public void ReplaceStatement(Statement statement)
    {
        if (statement == null) return;
        Statement originalStatement = FindMatchingStatement(statement);
        if (originalStatement == null)
            InsertStatementIntoList(statement);
        else
            ReplaceStatementInList(statement, originalStatement);
    }

    private void ReplaceStatementInList(Statement statement, Statement originalStatement)
    {
        if (_statements.Count == 1)
        {
            _statements.RemoveFirst();
            _statements.AddFirst(statement);
        }
        else
        {
            LinkedListNode<Statement> originalNode = _statements.Find(originalStatement);
            LinkedListNode<Statement> successor = originalNode!.Next;
            if (successor != null)
                _statements.AddBefore(successor!, statement);
            else
            {
                LinkedListNode<Statement> predecessor = originalNode.Previous;
                if (predecessor != null) _statements.AddAfter(predecessor, statement);
            }

            _statements.Remove(originalStatement);
        }
    }

    public Statement GetFirstStatement()
    {
        return _statements.FirstOrDefault();
    }
}