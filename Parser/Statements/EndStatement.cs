using System;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class EndStatement : IStatement
    {

        private readonly INotifier _notifier;

        public EndStatement(INotifier notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }
        public void Execute()
        {
            _notifier.Notify(this, Notification.EndProgram);
        }
    }
}