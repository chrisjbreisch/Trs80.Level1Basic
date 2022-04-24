using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class On : Statement
    {
        public Expression Selector { get; }
        public List<Expression> Locations { get; }
        public bool IsGosub { get; }

        public On(Expression selector, List<Expression> locations, bool isGosub)
        {
            Selector = selector;
            Locations = locations;
            IsGosub = isGosub;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitOnStatement(this);
        }
    }
}
