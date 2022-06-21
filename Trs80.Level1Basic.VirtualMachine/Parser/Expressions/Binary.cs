//
//
// This file was automatically generated by generateAst
// at 2022-06-20 11:38:10 AM UTC. Do not modify.
//
//

using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public class Binary : Expression
{
    public Expression Left { get; init; }
    public Token BinaryOperator { get; init; }
    public Expression Right { get; init; }

    public Binary(Expression left, Token binaryOperator, Expression right)
    {
        Left = left;
        BinaryOperator = binaryOperator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }
}
