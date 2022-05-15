//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Next : Statement
{
    public Expression Variable { get; }

    public Next(Expression variable)
    {
        Variable = variable;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitNextStatement(this);
    }
}
