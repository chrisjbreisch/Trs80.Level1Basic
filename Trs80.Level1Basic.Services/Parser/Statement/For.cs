using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class For : Statement
    {
        public Token Name { get; }
        public Expression.Expression StartValue { get; }
        public Expression.Expression EndValue { get; }
        public Expression.Expression StepValue { get; }

        public For(Token name, Expression.Expression startValue, Expression.Expression endValue, Expression.Expression stepValue)
        {
            Name = name;
            StartValue = startValue;
            EndValue = endValue;
            StepValue = stepValue;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitForStatement(this);
        }
    }
}
