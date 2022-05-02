//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements;

public class Goto : Statement
{
    public Expression Location { get; }

    public Goto(Expression location)
    {
        Location = location;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitGotoStatement(this);
    }
}
