using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class OnError : Statement
{
    public Expression Location { get; init; }

    public OnError(Expression location)
    {
        Location = location;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitOnErrorStatement(this);
}