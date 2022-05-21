//
//
// This file was automatically generated by generateAst
// at 2022-05-21 1:41:41 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Goto : Statement
{
    public Expression Location { get; init; }

    public Goto(Expression location)
    {
        Location = location;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGotoStatement(this);
    }
}
