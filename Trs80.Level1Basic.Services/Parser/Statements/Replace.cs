using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Replace : Statement
    {
        public Line Line { get; }

        public Replace(Line line)
        {
            Line = line;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitReplaceStatement(this);
        }
    }
}
