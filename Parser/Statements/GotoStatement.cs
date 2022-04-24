using System;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class GotoStatement : IStatement
    {
        private readonly INotifier _notifier;
        private readonly short _nextLine;
        public GotoStatement(short nextLine, INotifier notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _nextLine = nextLine;
        } 
        public void Execute()
        {
            _notifier.Notify(this, Notification.Goto, _nextLine);
        }
    }
}
