//
//
// This file was automatically generated by generateAst
// at 2022-06-21 2:33:37 AM UTC. Do not modify.
//
//

using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Rem : Statement
{
    public Literal Remark { get; init; }

    public Rem(Literal remark)
    {
        Remark = remark;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitRemStatement(this);
    }
}
