//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Parser.Expressions;

public abstract class Expression
{
    public abstract dynamic Accept(IExpressionVisitor visitor);
}
