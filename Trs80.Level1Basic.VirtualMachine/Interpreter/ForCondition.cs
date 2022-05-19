using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class ForCondition
{
    public Expression Identifier { get; set; }
    public int Step { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public IStatement Goto { get; set; }
}