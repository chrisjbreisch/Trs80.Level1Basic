using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class Save : Statement
    {
        public Expression.Expression Path { get; }

        public Save(Expression.Expression path)
        {
            Path = path;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitSaveStatement(this);
        }
    }
}
