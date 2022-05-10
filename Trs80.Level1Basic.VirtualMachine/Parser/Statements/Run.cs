//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Run : Statement
{
    public Expression StartAtLineNumber { get; }

    public Run(Expression startAtLineNumber)
    {
        StartAtLineNumber = startAtLineNumber;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitRunStatement(this);
    }
}