//
//
// This file was automatically generated by generateAst
// at 2022-05-19 6:42:09 PM UTC. Do not modify.
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
