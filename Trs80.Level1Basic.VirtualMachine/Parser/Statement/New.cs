using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class New : Statement
    {

        public New()
        {
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitNewStatement(this);
        }
    }
}
