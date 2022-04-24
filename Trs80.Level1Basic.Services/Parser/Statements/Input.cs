using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Input : Statement
    {
        public List<Expression> Expressions { get; }
        public bool WriteNewline { get; }

        public Input(List<Expression> expressions, bool writeNewline)
        {
            Expressions = expressions;
            WriteNewline = writeNewline;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitInputStatement(this);
        }
    }
}
