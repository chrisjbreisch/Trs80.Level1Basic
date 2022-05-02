//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Parser.Expressions;

public class Unary : Expression
{
    public Token OperatorType { get; }
    public Expression Right { get; }

    public Unary(Token operatorType, Expression right)
    {
        OperatorType = operatorType;
        Right = right;
    }

    public override dynamic Accept(IExpressionVisitor visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }
}