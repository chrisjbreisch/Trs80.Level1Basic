using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Delete : Statement
    {
        public int LineToDelete { get; }

        public Delete(int lineToDelete)
        {
            LineToDelete = lineToDelete;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitDeleteStatement(this);
        }
    }
}
