using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Return : Statement
    {

        public Return()
        {
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitReturnStatement(this);
        }
    }
}
