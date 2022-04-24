using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class List : Statement
    {
        public Expression.Expression StartAtLineNumber { get; }

        public List(Expression.Expression startAtLineNumber)
        {
            StartAtLineNumber = startAtLineNumber;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitListStatement(this);
        }
    }
}
