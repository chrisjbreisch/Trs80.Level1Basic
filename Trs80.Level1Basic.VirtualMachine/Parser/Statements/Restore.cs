//
//
// This file was automatically generated by generateAst
// at 2022-05-21 1:41:41 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Restore : Statement
{

    public Restore()
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitRestoreStatement(this);
    }
}
