//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Parser.Expressions;

public class Assign : Expression
{
    public Token Name { get; }
    public Expression Value { get; }

    public Assign(Token name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public override dynamic Accept(IExpressionVisitor visitor)
    {
        return visitor.VisitAssignExpression(this);
    }
}
