//
//
// This file was automatically generated by generateAst
// at 2022-06-11 12:25:56 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Cont : Statement
{

    public Cont()
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitContStatement(this);
    }
}
