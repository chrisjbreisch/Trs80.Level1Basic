//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.Interpreter.Parser.Expressions;

public class Literal : Expression
{
    public dynamic Value { get; }

    public Literal(dynamic value)
    {
        Value = value;
    }

    public override dynamic Accept(IExpressionVisitor visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}