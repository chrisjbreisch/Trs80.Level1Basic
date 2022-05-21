//
//
// This file was automatically generated by generateAst
// at 2022-05-21 1:41:41 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Identifier : Expression
{
    public Token Name { get; init; }

    public Identifier(Token name)
    {
        Name = name;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIdentifierExpression(this);
    }
}
