using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class List : Statement
    {
        public Expression StartAtLineNumber { get; }

        public List(Expression startAtLineNumber)
        {
            StartAtLineNumber = startAtLineNumber;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitListStatement(this);
        }
    }
}
