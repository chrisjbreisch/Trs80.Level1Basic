//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Call : Expression
{
    public Token Name { get; }
    public List<Expression> Arguments { get; }

    public Call(Token name, List<Expression> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitCallExpression(this);
    }
}
