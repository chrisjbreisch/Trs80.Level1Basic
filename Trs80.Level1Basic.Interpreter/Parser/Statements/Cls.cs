//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;

namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public class Cls : Statement
{

    public Cls()
    {
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitClsStatement(this);
    }
}