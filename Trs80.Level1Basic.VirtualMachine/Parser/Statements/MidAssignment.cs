using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class MidAssignment : Statement
{
    public Identifier Target { get; init; }
    public Expression Start { get; init; }
    public Expression Length { get; init; }
    public Expression Value { get; init; }

    public MidAssignment(Identifier target, Expression start, Expression length, Expression value)
    {
        Target = target;
        Start = start;
        Length = length;
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitMidAssignmentStatement(this);
    }
}
