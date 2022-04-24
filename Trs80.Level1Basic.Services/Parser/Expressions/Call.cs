using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expressions
{
    public class Call : Expression
    {
        public Token Name { get; }
        public List<Expression> Arguments { get; }

        public Call(Token name, List<Expression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitCallExpression(this);
        }
    }
}
