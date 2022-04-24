using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class Rem : Statement
    {
        public Expression.Literal Remark { get; }

        public Rem(Expression.Literal remark)
        {
            Remark = remark;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitRemStatement(this);
        }
    }
}
