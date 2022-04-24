using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Next : Statement
    {
        public Expression Variable { get; }

        public Next(Expression variable)
        {
            Variable = variable;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitNextStatement(this);
        }
    }
}
