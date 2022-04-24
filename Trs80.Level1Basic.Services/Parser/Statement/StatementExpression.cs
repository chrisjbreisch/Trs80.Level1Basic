using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class StatementExpression : Statement
    {
        public Expression.Expression Expression { get; }

        public StatementExpression(Expression.Expression expression)
        {
            Expression = expression;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitStatementExpressionStatement(this);
        }
    }
}
