//
//
// This file was automatically generated by generateAst
// at 2022-05-18 1:06:52 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class New : Statement
{

    public New()
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitNewStatement(this);
    }
}
