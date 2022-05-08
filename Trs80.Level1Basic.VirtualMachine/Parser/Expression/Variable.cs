using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expression
{
    public class Variable : Expression
    {
        public Token Name { get; }

        public Variable(Token name)
        {
            Name = name;
        }

        public override dynamic Accept(IExpressionVisitor visitor)
        {
            return visitor.VisitVariableExpression(this);
        }
    }
}
