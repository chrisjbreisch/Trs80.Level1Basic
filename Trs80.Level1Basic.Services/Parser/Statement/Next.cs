using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public class Next : Statement
    {
        public Token Name { get; }

        public Next(Token name)
        {
            Name = name;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.VisitNextStatement(this);
        }
    }
}
