using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Print : Statement
    {
        public Expression AtPosition { get; }
        public List<Expression> Expressions { get; }
        public bool WriteNewline { get; }

        public Print(Expression atPosition, List<Expression> expressions, bool writeNewline)
        {
            AtPosition = atPosition;
            Expressions = expressions;
            WriteNewline = writeNewline;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitPrintStatement(this);
        }
    }
}
