using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expression
{
    public class Assign : Expression
    {
        public Token Name { get; }
        public Expression Value { get; }

        public Assign(Token name, Expression value)
        {
            Name = name;
            Value = value;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitAssignExpression(this);
        }
    }
}
