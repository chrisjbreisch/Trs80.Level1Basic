//
//
// This file was automatically generated by generateAst
// at 2022-05-19 6:42:09 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public abstract class Statement: IStatement
{
    public int LineNumber { get; set; } = -1;
    public string SourceLine { get; set; }
    public IStatement Next { get; set; }
    public IStatement Previous { get; set; }
    public IStatement Parent { get; set; }

    public abstract T Accept<T>(IVisitor<T> visitor);
}
