using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class StatementExpression : Statement
    {
        public Expression Expression { get; }

        public StatementExpression(Expression expression)
        {
            Expression = expression;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitStatementExpressionStatement(this);
        }
    }
}
