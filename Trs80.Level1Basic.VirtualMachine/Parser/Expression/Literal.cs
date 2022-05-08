using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expression
{
    public class Literal : Expression
    {
        public dynamic Value { get; }

        public Literal(dynamic value)
        {
            Value = value;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitLiteralExpression(this);
        }
    }
}
