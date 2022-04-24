using System;

namespace Trs80.Level1Basic.Services
{
    public interface INotifier
    {
        event EventHandler<EnvironmentNotificationEventArgs> EnvironmentNotification;
        void Notify(object sender, Notification notification, short lineNumber = 0);
    }
}