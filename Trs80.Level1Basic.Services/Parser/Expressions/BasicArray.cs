using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expressions
{
    public class BasicArray : Expression
    {
        public Token Name { get; }
        public Expression Index { get; }

        public BasicArray(Token name, Expression index)
        {
            Name = name;
            Index = index;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitBasicArrayExpression(this);
        }
    }
}
