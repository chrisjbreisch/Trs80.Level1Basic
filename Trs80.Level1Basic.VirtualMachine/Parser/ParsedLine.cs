using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Parser;

public class ParsedLine
{
    public int LineNumber { get; set; } = -1;
    public string SourceLine { get; set; }
    public List<Statement> Statements { get; set; }
}