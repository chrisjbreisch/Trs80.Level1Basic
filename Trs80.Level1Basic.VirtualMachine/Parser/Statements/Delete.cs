//
//
// This file was automatically generated by generateAst
// at 2022-06-20 11:38:10 AM UTC. Do not modify.
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
