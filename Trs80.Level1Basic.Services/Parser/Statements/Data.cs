using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class Data : Statement
    {
        public List<Expression> DataElements { get; }

        public Data(List<Expression> dataElements)
        {
            DataElements = dataElements;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitDataStatement(this);
        }
    }
}
