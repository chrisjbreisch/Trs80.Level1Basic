//
//
// This file was automatically generated by generateAst
// at 2022-06-21 2:33:37 AM UTC. Do not modify.
//
//

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Return : Statement
{
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitReturnStatement(this);
    }
}
