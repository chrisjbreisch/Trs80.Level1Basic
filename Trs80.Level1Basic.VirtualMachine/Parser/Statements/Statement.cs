//
//
// This file was automatically generated by generateAst
// at 2022-05-15 9:07:31 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public abstract class Statement
{
    public int LineNumber { get; set; } = -1;
    public string SourceLine { get; set; }
    public Statement Next { get; set; }

    public abstract T Accept<T>(IVisitor<T> visitor);
}
