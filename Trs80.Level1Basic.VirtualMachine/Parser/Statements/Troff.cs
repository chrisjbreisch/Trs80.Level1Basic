namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Troff : Statement
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTroffStatement(this);
}