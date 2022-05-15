//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Data : Statement
{
    public List<Expression> DataElements { get; }

    public Data(List<Expression> dataElements)
    {
        DataElements = dataElements;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitDataStatement(this);
    }
}
