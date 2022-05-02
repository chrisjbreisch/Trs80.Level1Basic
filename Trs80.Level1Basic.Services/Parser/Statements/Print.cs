//
// This file is automatically generated. Do not modify.
//

using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;

namespace Trs80.Level1Basic.Services.Parser.Statements;

public class Print : Statement
{
    public Expression AtPosition { get; }
    public Expression TabPosition { get; }
    public List<Expression> Expressions { get; }
    public bool WriteNewline { get; }

    public Print(Expression atPosition, Expression tabPosition, List<Expression> expressions, bool writeNewline)
    {
        AtPosition = atPosition;
        TabPosition = tabPosition;
        Expressions = expressions;
        WriteNewline = writeNewline;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitPrintStatement(this);
    }
}
