using System;

namespace Trs80.Level1Basic.Services;

public class Notifier : INotifier
{
    public event EventHandler<EnvironmentNotificationEventArgs> EnvironmentNotification;

    public void Notify(object sender, Notification notification, short lineNumber = 0)
    {
        var eventArgs = new EnvironmentNotificationEventArgs {Notification = notification, LineNumber = lineNumber};
        OnEnvironmentNotification(sender, eventArgs);
    }
    protected void OnEnvironmentNotification(object sender, EnvironmentNotificationEventArgs e)
    {
        EnvironmentNotification?.Invoke(sender, e);
    }
}

public enum Notification
{
    EndProgram,
    Goto,
    GotoSuccessor
}
public class EnvironmentNotificationEventArgs : EventArgs
{
    public Notification Notification { get; set; }
    public short LineNumber { get; set; }
}