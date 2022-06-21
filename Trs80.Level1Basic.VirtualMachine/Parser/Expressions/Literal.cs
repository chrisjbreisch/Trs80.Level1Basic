//
//
// This file was automatically generated by generateAst
// at 2022-06-20 11:38:10 AM UTC. Do not modify.
//
//

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Literal : Expression
{
    public dynamic Value { get; init; }
    public string UpperValue { get; init; }

    public Literal(dynamic value, string upperValue)
    {
        Value = value;
        UpperValue = upperValue;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}
