//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;

namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public abstract class Statement
{
    public int LineNumber { get; set; } = -1;
    public string SourceLine { get; set; }
    public Statement Next { get; set; }

    public abstract void Accept(IStatementVisitor visitor);
}
