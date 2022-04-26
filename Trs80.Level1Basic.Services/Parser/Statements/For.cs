//
// This file is automatically generated. Do not modify.
//

using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statements
{
    public class For : Statement
    {
        public Expression Variable { get; }
        public Expression StartValue { get; }
        public Expression EndValue { get; }
        public Expression StepValue { get; }

        public For(Expression variable, Expression startValue, Expression endValue, Expression stepValue)
        {
            Variable = variable;
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
