using System;

namespace Trs80.Level1Basic.Parser.Expressions
{
    public class AssignmentExpression : Expression
    {

        private readonly IVariables _variables;
        public AssignmentExpression(IVariables variables, 
            VariableExpression name, IExpression value) : base (name, value)
        {
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        protected override dynamic Evaluate()
        {
            var value = Right.Value;
            //if (!IsCalculated)
            {
                //Value = Right.Value;
                //IsCalculated = true;
                _variables.SetValue(((VariableExpression)Left).Name, value);
            }
            return value;
        }

        public void Execute()
        {
            Evaluate();
        }
    }
}