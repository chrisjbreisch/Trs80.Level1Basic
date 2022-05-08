using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expression
{
    public interface IExpressionVisitor
    {
        dynamic VisitAssignExpression(Assign root);
        dynamic VisitBinaryExpression(Binary root);
        dynamic VisitGroupingExpression(Grouping root);
        dynamic VisitLiteralExpression(Literal root);
        dynamic VisitUnaryExpression(Unary root);
        dynamic VisitVariableExpression(Variable root);
    }

    public abstract class Expression
    {
        public abstract dynamic Accept(IExpressionVisitor visitor);
    }






}
