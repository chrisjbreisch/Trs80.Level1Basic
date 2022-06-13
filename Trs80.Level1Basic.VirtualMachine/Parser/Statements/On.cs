//
//
// This file was automatically generated by generateAst
// at 2022-06-13 12:03:06 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class On : Statement
{
    public Expression Selector { get; init; }
    public List<Expression> Locations { get; init; }
    public bool IsGosub { get; init; }

    public On(Expression selector, List<Expression> locations, bool isGosub)
    {
        Selector = selector;
        Locations = locations;
        IsGosub = isGosub;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitOnStatement(this);
    }
}
