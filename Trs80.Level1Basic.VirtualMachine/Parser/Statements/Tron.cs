namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Tron : Statement
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTronStatement(this);
}