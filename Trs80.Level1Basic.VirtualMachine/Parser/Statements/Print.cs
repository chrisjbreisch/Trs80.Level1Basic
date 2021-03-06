//
//
// This file was automatically generated by generateAst
// at 2022-06-26 3:23:04 PM UTC. Do not modify.
//
//

using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Print : Statement
{
    public Expression AtPosition { get; init; }
    public List<Expression> Expressions { get; init; }
    public bool WriteNewline { get; init; }

    public Print(Expression atPosition, List<Expression> expressions, bool writeNewline)
    {
        AtPosition = atPosition;
        Expressions = expressions;
        WriteNewline = writeNewline;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitPrintStatement(this);
    }
}
