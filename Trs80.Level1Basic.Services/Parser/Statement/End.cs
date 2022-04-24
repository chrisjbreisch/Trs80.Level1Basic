using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class End : Statement
    {

        public End()
        {
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitEndStatement(this);
        }
    }
}
