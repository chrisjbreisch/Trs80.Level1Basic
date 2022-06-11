//
//
// This file was automatically generated by generateAst
// at 2022-06-11 11:04:28 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Let : Statement
{
    public Expression Variable { get; init; }
    public Expression Initializer { get; init; }

    public Let(Expression variable, Expression initializer)
    {
        Variable = variable;
        Initializer = initializer;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLetStatement(this);
    }
}
