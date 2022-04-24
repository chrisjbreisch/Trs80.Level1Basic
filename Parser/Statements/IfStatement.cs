using System;
using Trs80.Level1Basic.Parser.Expressions;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class IfStatement : IStatement
    {
        private readonly IExpression _condition;
        private readonly INotifier _notifier;
        private readonly short _nextLine;
        public IfStatement(IExpression condition, short nextLine, INotifier notifier)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _nextLine = nextLine;
        } 
        public void Execute()
        {
            if (_condition.Value)
                _notifier.Notify(this, Notification.Goto, _nextLine);
        }
    }
}
