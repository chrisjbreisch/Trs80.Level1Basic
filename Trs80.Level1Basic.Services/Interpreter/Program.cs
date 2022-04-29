using System.Collections.Generic;
using System.Linq;
using Trs80.Level1Basic.Services.Parser;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter;

public class Program : IProgram
{
    private List<ParsedLine> _programLines = new();
    private readonly List<Statement> _programStatements = new();

    public void Initialize()
    {
        _programStatements.Clear();
        _sorted = false;
        Sort();

        Statement last = null;
        foreach (var statement in _programLines.SelectMany(line => line.Statements))
        {
            if (last != null)
                last.Next = statement;
            _programStatements.Add(statement);
            last = statement;
        }
    }

    public Statement GetExecutableStatement(int lineNumber)
    {
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

    public void RemoveLine(ParsedLine line)
    {
        _programLines.Remove(line);
    }

    public int Size()
    {
        return _programLines.Sum(statement => 4 + statement.SourceLine.Length);
    }

    public void AddLine(ParsedLine line)
    {
        _programLines.Add(line);
        _sorted = false;
    }

    private bool _sorted;
    public void ReplaceLine(ParsedLine line)
    {
        var programLine = _programLines.FirstOrDefault(l => l.LineNumber == line.LineNumber);
        if (programLine != null)
            programLine.SourceLine = line.SourceLine;
        else
        {
            _programLines.Add(line);
            _sorted = false;
            Sort();
        }
    }

    public void Sort()
    {
        if (_sorted) return;

        _programLines = _programLines.OrderBy(l => l.LineNumber).ToList();
        _sorted = true;
    }
}