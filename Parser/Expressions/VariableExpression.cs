using System;

namespace Trs80.Level1Basic.Parser.Expressions
{
    public class VariableExpression : Expression, ILeafExpression
    {
        private readonly IVariables _variables;
        public VariableExpression(IVariables variables, 
            string name) : base(new LiteralExpression<string>(name), new NullExpression())
        {
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        public string Name => Left.Value.ToString();

        protected override dynamic Evaluate()
        {
            return _variables.GetValue(Name);
        }
    }
}