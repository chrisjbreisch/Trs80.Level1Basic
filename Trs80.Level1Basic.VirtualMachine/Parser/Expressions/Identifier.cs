//
//
// This file was automatically generated by generateAst
// at 2022-06-11 12:25:56 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Identifier : Expression
{
    public Token Name { get; init; }
    public bool IsString { get; init; }
    public string LowerName { get; init; }

    public Identifier(Token name, bool isString, string lowerName)
    {
        Name = name;
        IsString = isString;
        LowerName = lowerName;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIdentifierExpression(this);
    }
}
