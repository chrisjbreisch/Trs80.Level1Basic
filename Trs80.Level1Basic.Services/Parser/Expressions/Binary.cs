using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expressions
{
    public class Binary : Expression
    {
        public Expression Left { get; }
        public Token OperatorType { get; }
        public Expression Right { get; }

        public Binary(Expression left, Token operatorType, Expression right)
        {
            Left = left;
            OperatorType = operatorType;
            Right = right;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }
    }
}
