//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Read : Statement
{
    public List<Expression> Variables { get; }

    public Read(List<Expression> variables)
    {
        Variables = variables;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitReadStatement(this);
    }
}
