namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Resume : Statement
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitResumeStatement(this);
}