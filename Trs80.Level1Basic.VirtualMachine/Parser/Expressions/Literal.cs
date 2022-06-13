//
//
// This file was automatically generated by generateAst
// at 2022-06-13 12:03:05 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Literal : Expression
{
    public dynamic Value { get; init; }

    public Literal(dynamic value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}
