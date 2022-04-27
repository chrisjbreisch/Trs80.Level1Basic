using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter;

public class ForCheckCondition
{
    public Expression Variable { get; set; }
    public int Step { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public Statement Next { get; set; }
}