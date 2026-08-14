using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Dim : Statement
{
    public List<Expression> Dimensions { get; init; }

    public Dim(List<Expression> dimensions)
    {
        Dimensions = dimensions;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitDimStatement(this);
    }
}
