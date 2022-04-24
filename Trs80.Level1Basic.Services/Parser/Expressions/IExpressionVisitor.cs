namespace Trs80.Level1Basic.Services.Parser.Expressions
{
    public interface IExpressionVisitor
    {
        dynamic VisitBasicArrayExpression(BasicArray root);
        dynamic VisitAssignExpression(Assign root);
        dynamic VisitBinaryExpression(Binary root);
        dynamic VisitCallExpression(Call root);
        dynamic VisitGroupingExpression(Grouping root);
        dynamic VisitLiteralExpression(Literal root);
        dynamic VisitUnaryExpression(Unary root);
        dynamic VisitIdentifierExpression(Identifier root);
    }
}
