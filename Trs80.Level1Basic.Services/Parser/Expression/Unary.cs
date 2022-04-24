using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expression
{
    public class Unary : Expression
    {
        public Token OperatorType { get; }
        public Expression Right { get; }

        public Unary(Token operatorType, Expression right)
        {
            OperatorType = operatorType;
            Right = right;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitUnaryExpression(this);
        }
    }
}
