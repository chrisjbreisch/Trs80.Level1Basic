using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class If : Statement
    {
        public Expression.Expression Condition { get; }
        public Expression.Expression GotoLineNumber { get; }

        public If(Expression.Expression condition, Expression.Expression gotoLineNumber)
        {
            Condition = condition;
            GotoLineNumber = gotoLineNumber;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }
    }
}
