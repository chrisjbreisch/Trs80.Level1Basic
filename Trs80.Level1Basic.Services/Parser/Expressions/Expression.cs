using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Expressions
{
    public abstract class Expression
    {
        public abstract dynamic Accept(IExpressionVisitor visitor);
    }
}
