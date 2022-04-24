using System;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class ForStatement : IStatement
    {
        private readonly IVariables _variables;
        private readonly IForCheckConditions _checkConditions;
        private readonly ForCheckCondition _checkCondition;

        public ForStatement(IForCheckConditions forCheckConditions, ForCheckCondition checkCondition, IVariables variables)
        {
            _checkConditions = forCheckConditions ?? throw new ArgumentNullException(nameof(forCheckConditions));
            _checkCondition = checkCondition ?? throw new ArgumentNullException(nameof(checkCondition));
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }
        public void Execute()
        {
            _checkConditions.Push(_checkCondition);
            _variables.SetValue(_checkCondition.VariableName, _checkCondition.StartValue.Value);

        }
    }
}
