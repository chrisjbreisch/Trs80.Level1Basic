using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Cont : Statement
    {

        public Cont()
        {
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitContStatement(this);
        }
    }
}
