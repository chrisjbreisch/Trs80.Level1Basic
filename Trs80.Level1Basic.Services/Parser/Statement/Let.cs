using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class Let : Statement
    {
        public Token Name { get; }
        public Expression.Expression Initializer { get; }

        public Let(Token name, Expression.Expression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitLetStatement(this);
        }
    }
}
