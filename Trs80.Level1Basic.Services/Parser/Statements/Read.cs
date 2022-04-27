//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements;

public class Read : Statement
{
    public List<Expression> Variables { get; }

    public Read(List<Expression> variables)
    {
        Variables = variables;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitReadStatement(this);
    }
}