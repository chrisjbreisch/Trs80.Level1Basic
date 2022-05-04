using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Parser.Statements;

namespace Trs80.Level1Basic.Interpreter.Interpreter;

public class Program : IProgram
{
    private List<ParsedLine> _programLines = new();
    private readonly List<Statement> _programStatements = new();
    private bool _sorted;

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
        IEnumerable<Statement> previousLines = _programLines.SelectMany(s => s.Statements).Where(p => p?.Next?.LineNumber == line.LineNumber);

        foreach (Statement previousLine in previousLines)
            previousLine.Next = line.Statements[0].Next;

        _programLines.Remove(line);
    }

    public int Size()
    {
        return _programLines.Sum(statement => 4 + statement.SourceLine.Length);
    }

    public void AddLine(ParsedLine line)
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

    public void Sort()
    {
        if (_sorted) return;

        _programLines = _programLines.OrderBy(l => l.LineNumber).ToList();
        _sorted = true;
    }
}