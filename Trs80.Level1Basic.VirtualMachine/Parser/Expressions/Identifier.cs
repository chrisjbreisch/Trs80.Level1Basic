//
//
// This file was automatically generated by generateAst
// at 2022-06-21 2:33:37 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Identifier : Expression
{
    public Token Name { get; init; }
    public bool IsString { get; init; }
    public int LinePosition { get; init; }

    public Identifier(Token name, bool isString, int linePosition)
    {
        Name = name;
        IsString = isString;
        LinePosition = linePosition;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIdentifierExpression(this);
    }
}
