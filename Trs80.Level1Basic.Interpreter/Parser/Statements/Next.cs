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

public class Next : Statement
{
    public Expression Variable { get; }

    public Next(Expression variable)
    {
        Variable = variable;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitNextStatement(this);
    }
}
