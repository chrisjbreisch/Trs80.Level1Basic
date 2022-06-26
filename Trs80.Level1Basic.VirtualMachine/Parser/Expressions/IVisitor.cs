//
//
// This file was automatically generated by generateAst
// at 2022-06-26 12:32:01 AM UTC. Do not modify.
//
//
namespace Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

public interface IVisitor<out T>
{
    T VisitArrayExpression(Array expression);
    T VisitBinaryExpression(Binary expression);
    T VisitCallExpression(Call expression);
    T VisitGroupingExpression(Grouping expression);
    T VisitIdentifierExpression(Identifier expression);
    T VisitLiteralExpression(Literal expression);
    T VisitSelectorExpression(Selector expression);
    T VisitUnaryExpression(Unary expression);
}
