//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Rem : Statement
{
    public Literal Remark { get; }

    public Rem(Literal remark)
    {
        Remark = remark;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitRemStatement(this);
    }
}
