//
//
// This file was automatically generated by generateAst
// at 2022-05-18 1:06:52 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Array : Expression
{
    public Token Name { get; }
    public Expression Index { get; }

    public Array(Token name, Expression index)
    {
        Name = name;
        Index = index;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitArrayExpression(this);
    }
}
