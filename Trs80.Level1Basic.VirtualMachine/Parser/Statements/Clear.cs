using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Clear : Statement
{
    public Expression Length { get; init; }

    public Clear(Expression length = null)
    {
        Length = length;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitClearStatement(this);
    }
}
