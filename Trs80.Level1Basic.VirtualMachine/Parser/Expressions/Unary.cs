//
//
// This file was automatically generated by generateAst
// at 2022-05-17 6:40:24 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Unary : Expression
{
    public Token UnaryOperator { get; }
    public Expression Right { get; }

    public Unary(Token unaryOperator, Expression right)
    {
        UnaryOperator = unaryOperator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }
}
