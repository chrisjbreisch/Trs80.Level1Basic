using System;
using System.Linq;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class NextStatement : IStatement
    {
        private readonly IForCheckConditions _checkConditions;
        private ForCheckCondition _checkCondition;
        private readonly IVariables _variables;
        private readonly INotifier _notifier;
        private readonly string _variableName;

        public NextStatement(string variableName, IForCheckConditions forCheckConditions, IVariables variables, INotifier notifier)
        {
            _variableName = variableName;
            _checkConditions = forCheckConditions ?? throw new ArgumentNullException(nameof(forCheckConditions));

            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        private void GetCheckCondition()
        {
            if (string.IsNullOrEmpty(_variableName))
                _checkCondition = _checkConditions.Pop();
            else
            {
                _checkCondition = _checkConditions.FirstOrDefault(c => c.VariableName == _variableName);
                if (_checkCondition == null)
                    throw new InvalidOperationException();
                _checkConditions.Remove(_checkCondition);
            }
        }

        public void Execute()
        {
            if (_checkCondition == null)
                GetCheckCondition();

            var currentValue = _variables.GetValue(_checkCondition.VariableName);
            var nextValue = currentValue + _checkCondition.Step.Value;
            _variables.SetValue(_checkCondition.VariableName, nextValue);

            if (_checkCondition.Step.Value > 0)
            {
                if (nextValue <= _checkCondition.EndValue.Value)
                    _notifier.Notify(this, Notification.GotoSuccessor, _checkCondition.LineNumber);
                else
                    _checkCondition = null;
            }
            else
            {
                if (nextValue >= _checkCondition.EndValue.Value)
                    _notifier.Notify(this, Notification.GotoSuccessor, _checkCondition.LineNumber);
                else
                    _checkCondition = null;
            }
        }
    }
}
