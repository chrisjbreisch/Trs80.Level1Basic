//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class If : Statement
    {
        public Expression Condition { get; }
        public List<Statement> ThenStatements { get; }

        public If(Expression condition, List<Statement> thenStatements)
        {
            Condition = condition;
            ThenStatements = thenStatements;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }
    }
}
