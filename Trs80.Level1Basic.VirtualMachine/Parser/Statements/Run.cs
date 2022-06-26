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

public class Run : Statement
{
    public Expression StartAtLineNumber { get; init; }

    public Run(Expression startAtLineNumber)
    {
        StartAtLineNumber = startAtLineNumber;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitRunStatement(this);
    }
}
