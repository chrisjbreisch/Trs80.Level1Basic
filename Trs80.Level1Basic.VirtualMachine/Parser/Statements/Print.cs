//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Print : Statement
{
    public Expression AtPosition { get; }
    public List<Expression> Expressions { get; }
    public bool WriteNewline { get; }

    public Print(Expression atPosition, List<Expression> expressions, bool writeNewline)
    {
        AtPosition = atPosition;
        Expressions = expressions;
        WriteNewline = writeNewline;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitPrintStatement(this);
    }
}
