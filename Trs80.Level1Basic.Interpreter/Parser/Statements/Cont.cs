//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;

namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public class Cont : Statement
{

    public Cont()
    {
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitContStatement(this);
    }
}