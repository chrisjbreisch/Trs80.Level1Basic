//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public class Read : Statement
{
    public List<Expression> Variables { get; }

    public Read(List<Expression> variables)
    {
        Variables = variables;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitReadStatement(this);
    }
}
