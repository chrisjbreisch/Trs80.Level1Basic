//
//
// This file was automatically generated by generateAst
// at 2022-06-20 11:38:10 AM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Assign : Expression
{
    public Token Name { get; init; }
    public Expression Value { get; init; }
    public bool IsString { get; init; }

    public Assign(Token name, Expression value, bool isString)
    {
        Name = name;
        Value = value;
        IsString = isString;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitAssignExpression(this);
    }
}
