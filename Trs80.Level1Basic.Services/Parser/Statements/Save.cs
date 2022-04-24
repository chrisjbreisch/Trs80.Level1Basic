using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Save : Statement
    {
        public Expression Path { get; }

        public Save(Expression path)
        {
            Path = path;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitSaveStatement(this);
        }
    }
}
