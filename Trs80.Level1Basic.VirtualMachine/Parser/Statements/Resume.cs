namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Resume : Statement
{
    public Expression Location { get; init; }

    public Resume(Expression location = null)
    {
        Location = location;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitResumeStatement(this);
}