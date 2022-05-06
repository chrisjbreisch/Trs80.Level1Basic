using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Statements;

namespace Trs80.Level1Basic.Interpreter.Parser;

public class ParsedLine
{
    public int LineNumber { get; set; } = -1;
    public string SourceLine { get; set; }
    public List<Statement> Statements { get; set; }
}