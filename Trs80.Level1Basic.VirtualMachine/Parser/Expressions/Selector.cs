//
//
// This file was automatically generated by generateAst
// at 2022-06-26 3:23:04 PM UTC. Do not modify.
//
//

using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Selector : Expression
{
    public Expression Expression { get; init; }

    public Selector(Expression expression, int linePosition) : base(linePosition)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitSelectorExpression(this);
    }
}