//
//
// This file was automatically generated by generateAst
// at 2022-06-11 11:04:28 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Grouping : Expression
{
    public Expression Expression { get; init; }

    public Grouping(Expression expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpression(this);
    }
}
