using Notifications.Wpf;
using XmlHandler.Services.Interfaces;

namespace XmlHandler.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationManager _notificationManager = new();

    public void Show(string title, string message, NotificationType type)
    {
        _notificationManager.Show(new NotificationContent
        {
            Title = title,
            Message = message,
            Type = type
        });
    }

}
