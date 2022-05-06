//
//
// This file is automatically generated by generateAst. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;

namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public class StatementExpression : Statement
{
    public Expression Expression { get; }

    public StatementExpression(Expression expression)
    {
        Expression = expression;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitStatementExpressionStatement(this);
    }
}
