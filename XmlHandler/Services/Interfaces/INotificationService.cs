using Notifications.Wpf;

namespace XmlHandler.Services.Interfaces;

public interface INotificationService
{
    void Show(string title, string message, NotificationType type);
}
