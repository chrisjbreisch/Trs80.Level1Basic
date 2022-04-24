using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expressions
{
    public class Identifier : Expression
    {
        public Token Name { get; }

        public Identifier(Token name)
        {
            Name = name;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitIdentifierExpression(this);
        }
    }
}
