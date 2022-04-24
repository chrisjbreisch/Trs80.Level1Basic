using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expressions
{
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
}
