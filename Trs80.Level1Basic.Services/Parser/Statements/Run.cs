using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Run : Statement
    {
        public Expression StartAtLineNumber { get; }

        public Run(Expression startAtLineNumber)
        {
            StartAtLineNumber = startAtLineNumber;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitRunStatement(this);
        }
    }
}
