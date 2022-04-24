using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class Print : Statement
    {
        public List<Expression.Expression> Expressions { get; }
        public bool WriteNewline { get; }
        public bool IsTabular { get; }

        public Print(List<Expression.Expression> expressions, bool writeNewline, bool isTabular)
        {
            Expressions = expressions;
            WriteNewline = writeNewline;
            IsTabular = isTabular;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitPrintStatement(this);
        }
    }
}
