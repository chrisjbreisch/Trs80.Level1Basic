//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public abstract class Statement
{
    public int LineNumber { get; set; }
    public string SourceLine { get; set; }
    public Guid UniqueIdentifier { get; set; }
    public Statement Next { get; set; }

    public abstract void Accept(IStatementVisitor visitor);
}
