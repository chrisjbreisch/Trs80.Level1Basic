using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Clear : Statement
{
    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitClearStatement(this);
    }
}
