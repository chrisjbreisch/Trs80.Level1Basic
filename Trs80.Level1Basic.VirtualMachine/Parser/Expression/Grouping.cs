using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expression
{
    public class Grouping : Expression
    {
        public Expression Expression { get; }

        public Grouping(Expression expression)
        {
            Expression = expression;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitGroupingExpression(this);
        }
    }
}
