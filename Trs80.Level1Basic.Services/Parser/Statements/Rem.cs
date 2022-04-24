using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Rem : Statement
    {
        public Literal Remark { get; }

        public Rem(Literal remark)
        {
            Remark = remark;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitRemStatement(this);
        }
    }
}
