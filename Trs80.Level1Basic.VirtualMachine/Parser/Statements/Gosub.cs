//
//
// This file was automatically generated by generateAst
// at 2022-06-21 2:33:37 AM UTC. Do not modify.
//
//

using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Gosub : Statement
{
    public Expression Location { get; init; }

    public Gosub(Expression location)
    {
        Location = location;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGosubStatement(this);
    }
}
