using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Let : Statement
    {
        public Expression Variable { get; }
        public Expression Initializer { get; }

        public Let(Expression variable, Expression initializer)
        {
            Variable = variable;
            Initializer = initializer;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitLetStatement(this);
        }
    }
}
