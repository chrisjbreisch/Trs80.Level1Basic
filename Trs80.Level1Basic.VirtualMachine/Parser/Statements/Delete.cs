//
//
// This file was automatically generated by generateAst
// at 2022-05-20 9:46:39 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Delete : Statement
{
    public int LineToDelete { get; init; }

    public Delete(int lineToDelete)
    {
        LineToDelete = lineToDelete;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitDeleteStatement(this);
    }
}
