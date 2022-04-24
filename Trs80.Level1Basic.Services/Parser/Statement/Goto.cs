using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class Goto : Statement
    {
        public Expression.Expression GotoLineNumber { get; }

        public Goto(Expression.Expression gotoLineNumber)
        {
            GotoLineNumber = gotoLineNumber;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitGotoStatement(this);
        }
    }
}
